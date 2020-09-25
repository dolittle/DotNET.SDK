// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.SDK.Artifacts;
using Dolittle.SDK.Events.for_EventConverter.given;
using Dolittle.SDK.Protobuf;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.SDK.Events.for_EventStore
{
    public class when_committing_an_event_with_an_unknown_event_type : given.an_event_store_and_an_execution_context
    {
        static an_event content;
        static EventSourceId event_source;
        static EventType event_type;
        static Exception exception;

        Establish context = () =>
        {
            content = new an_event("goodbye world", 12345, true);
            event_source = new EventSourceId("e4799653-eb7d-481b-9548-3156ddb45832");
            event_type = new EventType("c4aa0398-e14a-4867-bd20-318b78d8ccaa");
            event_types.Setup(_ => _.GetFor(content.GetType())).Throws(new UnknownArtifact(content.GetType()));
        };

        Because of = () =>
        {
            try
            {
                event_store.Commit(content, event_source).Wait();
            }
            catch (Exception e)
            {
                exception = e;
            }
        };

        It should_thrown_an_unknkown_artifact_exception = () => exception.ShouldBeOfExactType<UnknownArtifact>();
        It should_not_call_the_converter_to_protobuf = () => converter.Verify(_ => _.ToProtobuf(Moq.It.IsAny<UncommittedEvents>()), Times.Never());
        It should_not_call_the_caller = () => caller.Verify(_ => _.Call(Moq.It.IsAny<EventStoreCommitMethod>(), commit_events_request, Moq.It.IsAny<CancellationToken>()), Times.Never());
        It should_not_call_the_converter_to_sdk = () => converter.Verify(_ => _.ToSDK(commit_events_response), Times.Never());
    }
}
