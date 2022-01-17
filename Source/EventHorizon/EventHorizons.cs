// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.EventHorizon.Internal;
using Dolittle.SDK.Execution;
using Dolittle.SDK.Failures;
using Dolittle.SDK.Protobuf;
using Dolittle.SDK.Services;
using Dolittle.Services.Contracts;
using Microsoft.Extensions.Logging;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;
using SubscriptionRequest = Dolittle.Runtime.EventHorizon.Contracts.Subscription;
using SubscriptionResponse = Dolittle.Runtime.EventHorizon.Contracts.SubscriptionResponse;

namespace Dolittle.SDK.EventHorizon;

/// <summary>
/// Represents an implementation of <see cref="IEventHorizons"/>.
/// </summary>
public class EventHorizons : IEventHorizons, IDisposable
{
    static readonly SubscriptionsSubscribeMethod _method = new();
    readonly Subject<Subscription> _subscriptions = new();
    readonly ReplaySubject<SubscribeResponse> _responses = new();
    readonly IPerformMethodCalls _caller;
    readonly ExecutionContext _executionContext;
    readonly EventSubscriptionRetryPolicy _retryEventSubscription;
    readonly ILogger _logger;
    bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventHorizons"/> class.
    /// </summary>
    /// <param name="caller">The method caller to use to perform calls to the Runtime.</param>
    /// <param name="executionContext">Tha base <see cref="ExecutionContext"/>.</param>
    /// <param name="retryEventSubscription"><see cref="EventSubscriptionRetryPolicy"/> for retrying if subscription failed or has an exception.</param>
    /// <param name="logger">The <see cref="ILogger"/> to use.</param>
    public EventHorizons(
        IPerformMethodCalls caller,
        ExecutionContext executionContext,
        EventSubscriptionRetryPolicy retryEventSubscription,
        ILogger logger)
    {
        _caller = caller;
        _executionContext = executionContext;
        _retryEventSubscription = retryEventSubscription;
        _logger = logger;

        SetupSubscriptionProcessing();
    }

    /// <inheritdoc/>
    public IObservable<SubscribeResponse> Responses => _responses;

    /// <inheritdoc/>
    public Task<SubscribeResponse> Subscribe(Subscription subscription)
    {
        _subscriptions.OnNext(subscription);
        return _responses.Where(_ => _.Subscription == subscription).FirstAsync().ToTask();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Dispose resources.
    /// </summary>
    /// <param name="disposeManagedResources">Whether to dispose managed resources.</param>
    protected virtual void Dispose(bool disposeManagedResources)
    {
        if (_disposed) return;

        if (disposeManagedResources)
        {
            _subscriptions.Dispose();
            _responses.Dispose();
        }

        _disposed = true;
    }

    void SetupSubscriptionProcessing()
    {
        _subscriptions
            .Select(subscription => (subscription, request: CreateRuntimeRequestFromSubscription(subscription)))
            .Select(_ => Observable.FromAsync((token) => ProcessSubscriptionRequest(_.subscription, _.request, token)))
            .Merge()
            .Subscribe(_responses);
    }

    SubscriptionRequest CreateRuntimeRequestFromSubscription(Subscription subscription)
    {
        var subscriptionExecutionContext = _executionContext.ForTenant(subscription.ConsumerTenant);

        return new SubscriptionRequest
        {
            CallContext = new CallRequestContext
            {
                HeadId = Guid.Empty.ToProtobuf(),
                ExecutionContext = subscriptionExecutionContext.ToProtobuf(),
            },
            MicroserviceId = subscription.ProducerMicroservice.ToProtobuf(),
            TenantId = subscription.ProducerTenant.ToProtobuf(),
            StreamId = subscription.ProducerStream.ToProtobuf(),
            PartitionId = subscription.ProducerPartition.Value,
            ScopeId = subscription.ConsumerScope.ToProtobuf(),
        };
    }

    SubscribeResponse CreateResponseFromRuntimeResponse(Subscription subscription, SubscriptionResponse response) => new(subscription, response.ConsentId?.To<ConsentId>() ?? ConsentId.NotSet, response.Failure.ToSDK());

    async Task<SubscribeResponse> ProcessSubscriptionRequest(Subscription subscription, SubscriptionRequest request, CancellationToken cancellationToken)
    {
        SubscribeResponse response = null;
        await _retryEventSubscription(subscription, _logger, async () =>
        {
            try
            {
                _logger.LogDebug(
                    "Subscribing to events from {ProducerMicroservice} in {ProducerTenant} in {ProducerStream} in {ProducerPartition} for {ConsumerTenant} into {ConsumerScope}",
                    subscription.ProducerMicroservice,
                    subscription.ProducerTenant,
                    subscription.ProducerStream,
                    subscription.ProducerPartition,
                    subscription.ConsumerTenant,
                    subscription.ConsumerScope);

                var runtimeResponse = await _caller.Call(_method, request, cancellationToken).ConfigureAwait(false);
                response = CreateResponseFromRuntimeResponse(subscription, runtimeResponse);

                if (response.Failed)
                {
                    _logger.LogWarning(
                        "Failed to subscribe to events from {ProducerMicroservice} in {ProducerTenant} in {ProducerStream} in {ProducerPartition} for {ConsumerTenant} into {ConsumerScope} because {Reason}",
                        subscription.ProducerMicroservice,
                        subscription.ProducerTenant,
                        subscription.ProducerStream,
                        subscription.ProducerPartition,
                        subscription.ConsumerTenant,
                        subscription.ConsumerScope,
                        response.Failure.Reason);
                }
                else
                {
                    _logger.LogDebug(
                        "Successfully subscribed to events from {ProducerMicroservice} in {ProducerTenant} in {ProducerStream} in {ProducerPartition} for {ConsumerTenant} into {ConsumerScope} with {Consent}",
                        subscription.ProducerMicroservice,
                        subscription.ProducerTenant,
                        subscription.ProducerStream,
                        subscription.ProducerPartition,
                        subscription.ConsumerTenant,
                        subscription.ConsumerScope,
                        response.Consent);

                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "An exception was thrown while registering an event horizon subscription.");
            }

            return false;
        }).ConfigureAwait(false);

        return response;
    }
}