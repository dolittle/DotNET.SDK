// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using System.Threading;
using Dolittle.SDK.Services;
using Machine.Specifications;
using Microsoft.Extensions.Logging;
using Moq;
using static Dolittle.Runtime.Events.Contracts.EventStore;
using Contracts = Dolittle.Runtime.Events.Contracts;

namespace Dolittle.SDK.Events.for_EventStore.given
{
    public class an_event_store_and_an_execution_context : an_execution_context
    {
        protected static Mock<IPerformMethodCalls> caller;
        protected static Mock<IEventConverter> converter;
        protected static Mock<IEventTypes> event_types;
        protected static IEventStore event_store;
        protected static Contracts.CommitEventsRequest commit_events_request;
        protected static UncommittedEvents events_to_convert;

        Establish context = () =>
        {
            caller = new Mock<IPerformMethodCalls>();
            event_types = new Mock<IEventTypes>();
            converter = new Mock<IEventConverter>();

            event_store = new EventStore(caller.Object, converter.Object, execution_context, event_types.Object, Mock.Of<ILogger>());

            // we don't care here what the converter is doing, just return the correct type at least
            converter.Setup(_ => _.ToSDK(Moq.It.IsAny<Contracts.CommitEventsResponse>()))
                .Returns(Moq.It.IsAny<CommitEventsResult>);

            // allows us to inspect the uncommitted events just before they'd get committed and make it
            // return the correct type and amount at least
            converter.Setup(_ => _.ToProtobuf(Moq.It.IsAny<UncommittedEvents>()))
                .Callback<UncommittedEvents>((events) => events_to_convert = events)
                .Returns((UncommittedEvents @events) => @events.Select(_ => new Contracts.UncommittedEvent { }));

            // so that we can inspect the request to be committed
            caller.Setup(_ => _.Call(Moq.It.IsAny<EventStoreCommitMethod>(), Moq.It.IsAny<Contracts.CommitEventsRequest>(), Moq.It.IsAny<CancellationToken>()))
                .Callback<ICanCallAnUnaryMethod<EventStoreClient, Contracts.CommitEventsRequest, Contracts.CommitEventsResponse>, Contracts.CommitEventsRequest, CancellationToken>((method, request, token) =>
                {
                    commit_events_request = request;
                });
        };
    }
}
