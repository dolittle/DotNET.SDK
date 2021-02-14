// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Store;
using Machine.Specifications;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Dolittle.SDK.Aggregates.for_AggregateOf.given
{
    public abstract class all_dependencies
    {
        protected static Mock<IEventTypes> event_types;
        protected static ILoggerFactory logger_factory;
        protected static Mock<IEventStore> event_store;

        protected static void SetupCommittedEvents(AggregateRootId aggregate_root, EventSourceId event_source, params CommittedAggregateEvent[] events)
            => event_store.Setup(_ => _
                .FetchForAggregate(
                    aggregate_root,
                    event_source,
                    Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new CommittedAggregateEvents(event_source, aggregate_root, events)));

        Establish context = () =>
        {
            event_types = new Mock<IEventTypes>();
            logger_factory = NullLoggerFactory.Instance;
            event_store = new Mock<IEventStore>();
        };
    }
}
