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
using Dolittle.SDK.Protobuf;
using Dolittle.SDK.Services;
using Dolittle.Services.Contracts;
using Microsoft.Extensions.Logging;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;
using SubscriptionRequest = Dolittle.Runtime.EventHorizon.Contracts.Subscription;
using SubscriptionResponse = Dolittle.Runtime.EventHorizon.Contracts.SubscriptionResponse;

namespace Dolittle.SDK.EventHorizon
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventHorizons"/>.
    /// </summary>
    #pragma warning disable CA1001
    public class EventHorizons : IEventHorizons
    #pragma warning restore CA1001
    {
        static readonly SubscriptionsSubscribeMethod _method = new SubscriptionsSubscribeMethod();
        readonly Subject<Subscription> _subscriptions = new Subject<Subscription>();
        readonly ReplaySubject<SubscribeResponse> _responses = new ReplaySubject<SubscribeResponse>();
        readonly IPerformMethodCalls _caller;
        readonly ExecutionContext _executionContext;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHorizons"/> class.
        /// </summary>
        /// <param name="caller">The method caller to use to perform calls to the Runtime.</param>
        /// <param name="executionContext">Tha base <see cref="ExecutionContext"/>.</param>
        /// <param name="logger">The <see cref="ILogger"/> to use.</param>
        /// <param name="cancellationToken">Token that can be used to cancel this operation.</param>
        public EventHorizons(
            IPerformMethodCalls caller,
            ExecutionContext executionContext,
            ILogger logger,
            CancellationToken cancellationToken)
        {
            _caller = caller;
            _executionContext = executionContext;
            _logger = logger;

            SetupSubscriptionProcessing();
            SetupSubjectDisposal(cancellationToken);
        }

        /// <inheritdoc/>
        public IObservable<SubscribeResponse> Responses => _responses;

        /// <inheritdoc/>
        public Task<SubscribeResponse> Subscribe(Subscription subscription)
        {
            _subscriptions.OnNext(subscription);
            return _responses.Where(_ => _.Subscribtion == subscription).FirstAsync().ToTask();
        }

        void SetupSubscriptionProcessing()
        {
            _subscriptions
                .Select(subscription => (subscription, request: CreateRuntimeRequestFromSubscription(subscription)))
                .Select(_ => Observable.FromAsync((token) => ProcessSubscriptionRequest(_.subscription, _.request, token)))
                .Merge()
                .Subscribe(_responses);
        }

        void SetupSubjectDisposal(CancellationToken cancellationToken)
            => cancellationToken.Register(() =>
            {
                _subscriptions.Dispose();
                _responses.Dispose();
            });

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
                PartitionId = subscription.ProducerPartition.ToProtobuf(),
                ScopeId = subscription.ConsumerScope.ToProtobuf(),
            };
        }

        SubscribeResponse CreateResponseFromRuntimeResponse(Subscription subscription, SubscriptionResponse response)
            => new SubscribeResponse(subscription, response.ConsentId?.To<ConsentId>() ?? ConsentId.NotSet, response.Failure);

        async Task<SubscribeResponse> ProcessSubscriptionRequest(Subscription subscription, SubscriptionRequest request, CancellationToken cancellationToken)
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
                var response = CreateResponseFromRuntimeResponse(subscription, runtimeResponse);

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
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "An exception whas thrown while registering an event horizon subscription.");
                return new SubscribeResponse(subscription, ConsentId.NotSet, new Failure(FailureId.Undocumented, ex.Message));
            }
        }
    }
}
