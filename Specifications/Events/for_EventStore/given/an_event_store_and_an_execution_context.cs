// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Protobuf;
using Dolittle.SDK.Services;
using Machine.Specifications;
using Microsoft.Extensions.Logging;
using Moq;
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
        protected static Contracts.CommitEventsResponse commit_events_response;
        protected static IEnumerable<Contracts.UncommittedEvent> pb_uncommitted_events;
        protected static CommitEventsResult commit_events_result;

        Establish context = () =>
        {
            caller = new Mock<IPerformMethodCalls>();
            event_types = new Mock<IEventTypes>();
            converter = new Mock<IEventConverter>();

            event_store = new EventStore(caller.Object, converter.Object, execution_context, event_types.Object, Mock.Of<ILogger>());

            pb_uncommitted_events = new List<Contracts.UncommittedEvent>();
            pb_uncommitted_events.Append(new Contracts.UncommittedEvent());
            pb_uncommitted_events.Append(new Contracts.UncommittedEvent());

            converter.Setup(_ => _.ToProtobuf(Moq.It.IsAny<UncommittedEvents>()))
                .Returns(pb_uncommitted_events);

            var pb_committed_events = new List<Contracts.CommittedEvent>();
            pb_committed_events.Append(new Contracts.CommittedEvent());
            pb_committed_events.Append(new Contracts.CommittedEvent());

            commit_events_response = new Contracts.CommitEventsResponse();

            caller.Setup(_ => _.Call(Moq.It.IsAny<EventStoreCommitMethod>(), Moq.It.IsAny<Contracts.CommitEventsRequest>(), Moq.It.IsAny<CancellationToken>()))
                .Callback<ICanCallAUnaryMethod<Contracts.CommitEventsRequest, Contracts.CommitEventsResponse>, Contracts.CommitEventsRequest, CancellationToken>((method, request, token) => commit_events_request = request)
                .Returns(Task.FromResult(commit_events_response));

            var failure = new Failure(Guid.Parse("72ae75dc-cdd7-4413-bf19-66aba13486ad"), "ran out of tacos");
            var committedEvents = new CommittedEvents(new List<CommittedEvent>());
            commit_events_result = new CommitEventsResult(failure, committedEvents);

            converter.Setup(_ => _.ToSDK(commit_events_response))
                .Returns(commit_events_result);
        };
    }
}
