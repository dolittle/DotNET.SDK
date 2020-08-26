// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Execution;
using Dolittle.Heads;
using Dolittle.Lifecycle;
using Dolittle.Logging;
using Dolittle.Protobuf;
using Dolittle.Tenancy;
using static Dolittle.Runtime.EventHorizon.Contracts.Subscriptions;
using Contracts = Dolittle.Runtime.EventHorizon.Contracts;

namespace Dolittle.EventHorizon
{
    /// <summary>
    /// Represents an implementation of <see cref="ISubscriptions" />.
    /// </summary>
    [Singleton]
    public class Subscriptions : ISubscriptions
    {
        readonly SubscriptionsClient _client;
        readonly IExecutionContextManager _executionContextManager;
        readonly Head _head;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Subscriptions"/> class.
        /// </summary>
        /// <param name="client">The <see cref="SubscriptionsClient"/> to use for connecting to the Runtime.</param>
        /// <param name="executionContextManager">The <see cref="IExecutionContextManager"/> to use for getting the current <see cref="Execution.ExecutionContext"/>.</param>
        /// <param name="head">The current <see cref="Head"/>.</param>
        /// <param name="logger">The <see cref="ILogger"/> to use for logging.</param>
        public Subscriptions(
            SubscriptionsClient client,
            IExecutionContextManager executionContextManager,
            Head head,
            ILogger logger)
        {
            _client = client;
            _executionContextManager = executionContextManager;
            _head = head;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<SubscriptionResponse> Subscribe(TenantId consumerTenant, Subscription subscription, CancellationToken cancellationToken)
        {
            _executionContextManager.CurrentFor(consumerTenant);
            _logger.Debug("Subscribing to events from {Partition} in {Stream} of {ProducerTenant} in {Microservice} for {ConsumerTenant} into {Scope}", subscription.Partition, subscription.Stream, subscription.Tenant, subscription.Microservice, consumerTenant, subscription.Scope);
            var response = await _client.SubscribeAsync(
                new Contracts.Subscription
                {
                    CallContext = new Services.Contracts.CallRequestContext
                    {
                        HeadId = _head.Id.ToProtobuf(),
                        ExecutionContext = _executionContextManager.Current.ToProtobuf(),
                    },
                    PartitionId = subscription.Partition.ToProtobuf(),
                    StreamId = subscription.Stream.ToProtobuf(),
                    TenantId = subscription.Tenant.ToProtobuf(),
                    MicroserviceId = subscription.Microservice.ToProtobuf(),
                    ScopeId = subscription.Scope.ToProtobuf()
                }, cancellationToken: cancellationToken);
            return new SubscriptionResponse(response.ConsentId, response.Failure);
        }
    }
}