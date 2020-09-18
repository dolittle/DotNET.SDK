// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Protobuf;
using Machine.Specifications;
using Contracts = Dolittle.Runtime.Events.Contracts;

namespace Dolittle.SDK.Events.for_EventConverter.when_converting_to_protobuf
{
    public class with_a_valid_event : given.events
    {
        static Contracts.UncommittedEvent result;

        Because of = () => result = converter.ToProtobuf(uncomitted_event);

        It should_return_a_protobuf_uncommitted_event = () => result.ShouldBeOfExactType<Contracts.UncommittedEvent>();
        It should_have_the_same_event_type = () => result.Artifact.Id.Value.ShouldEqual(uncomitted_event.EventType.ToProtobuf().Id.Value);
        It should_have_same_event_source_id = () => result.EventSourceId.ShouldEqual(uncomitted_event.EventSource.ToProtobuf());
        It should_have_same_public = () => result.Public.ShouldEqual(uncomitted_event.IsPublic);
    }
}
