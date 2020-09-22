// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading;
using Dolittle.SDK.Events.for_EventConverter.given;
using Dolittle.SDK.Execution;
using Dolittle.SDK.Protobuf;
using Machine.Specifications;
using Contracts = Dolittle.Runtime.Events.Contracts;
using It = Machine.Specifications.It;

namespace Dolittle.SDK.Events.for_EventStore
{
    public class when_committing_uncommitted_events : given.an_event_store_and_an_execution_context
    {
        static UncommittedEvents uncommitted_events;

        Establish context = () =>
        {
            var @event = new an_event("goodbye world", 12345, false);
            uncommitted_events = new UncommittedEvents
                {
                    new UncommittedEvent(Guid.NewGuid(), new EventType(Guid.NewGuid()), @event, false),
                    new UncommittedEvent(Guid.NewGuid(), new EventType(Guid.NewGuid()), @event, false),
                    new UncommittedEvent(Guid.NewGuid(), new EventType(Guid.NewGuid()), @event, false),
                };
        };

        Because of = async () => await event_store.Commit(uncommitted_events);
        It should_have_added_events_to_the_request = () => commit_events_request.Events.Count.ShouldEqual(uncommitted_events.Count);
        It should_have_called_the_caller = () => caller.Verify(_ => _.Call(Moq.It.IsAny<EventStoreCommitMethod>(), Moq.It.IsAny<Contracts.CommitEventsRequest>(), Moq.It.IsAny<CancellationToken>()));
        It should_have_added_the_execution_context_to_the_call_context = () => commit_events_request.CallContext.ExecutionContext.ShouldEqual(execution_context.ToProtobuf());
        It should_have_added_a_not_set_head_id_to_the_call_context = () => commit_events_request.CallContext.HeadId.ShouldEqual(HeadId.NotSet.Value.ToProtobuf());
    }
}
