// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Artifacts;
using Dolittle.Protobuf;
using Dolittle.Serialization.Json;
using Google.Protobuf.WellKnownTypes;
using Machine.Specifications;
using Moq;
using given = Dolittle.Events.given;
using grpcArtifacts = Dolittle.Artifacts.Contracts;
using grpcEvents = Dolittle.Runtime.Events.Contracts;
using It = Machine.Specifications.It;

namespace Dolittle.Events.for_EventConverter
{
    public class when_converting_event_from_protobuf : given::Events
    {
        static Mock<IArtifactTypeMap> artifact_type_map;
        static Mock<ISerializer> serializer;
        static EventConverter converter;

        static CommittedEvent result;
        static grpcEvents.CommittedEvent input;

        static Guid artifact;
        static uint generation;
        static MyEvent the_event;
        static EventSourceId event_source;
        static DateTimeOffset occurred;
        static EventLogSequenceNumber event_log_sequence_number;

        Establish context = () =>
        {
            artifact_type_map = new Mock<IArtifactTypeMap>();
            serializer = new Mock<ISerializer>();

            converter = new EventConverter(artifact_type_map.Object, serializer.Object);

            artifact = Guid.Parse("0e5dce70-15f3-4e48-b0cc-ecb9dba50af4");
            generation = 42;
            the_event = new MyEvent();
            event_source = Guid.Parse("c15f86b2-4cc2-40d0-88e4-c016916fdddf");
            occurred = DateTimeOffset.UtcNow;
            event_log_sequence_number = 5;

            input = new grpcEvents.CommittedEvent
            {
                Type = new grpcArtifacts.Artifact
                {
                    Id = artifact.ToProtobuf(),
                    Generation = generation
                },
                ExecutionContext = execution_context.ToProtobuf(),
                EventSourceId = event_source.ToProtobuf(),
                Occurred = Timestamp.FromDateTimeOffset(occurred),
                EventLogSequenceNumber = event_log_sequence_number,
                Content = "{\"someProperty\":42}",
                Public = false,
                External = false,
            };

            artifact_type_map.Setup(_ => _.GetTypeFor(new Artifact(artifact, generation))).Returns(typeof(MyEvent));
            serializer.Setup(_ => _.FromJson(typeof(MyEvent), input.Content, SerializationOptions.CamelCase)).Returns(the_event);
        };

        Because of = () => result = converter.ToSDK(input);

        It should_be_a_committed_event = () => result.ShouldBeOfExactType<CommittedEvent>();
        It should_hold_the_event = () => result.Event.ShouldEqual(the_event);
        It should_have_the_correct_timestamp = () => result.Occurred.ShouldEqual(occurred);
        It should_have_the_correct_execution_context = () => result.ExecutionContext.ShouldEqual(execution_context);
        It should_have_the_correct_event_source = () => result.EventSource.ShouldEqual(event_source);
        It should_have_the_correct_event_log_sequence_number = () => result.EventLogSequenceNumber.ShouldEqual(event_log_sequence_number);
    }
}