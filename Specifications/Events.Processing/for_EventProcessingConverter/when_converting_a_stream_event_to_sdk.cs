// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using Dolittle.Protobuf.Contracts;
using Dolittle.SDK.Execution;
using Dolittle.SDK.Protobuf;
using Dolittle.SDK.Security;
using Machine.Specifications;
using PbCommittedEvent = Dolittle.Runtime.Events.Contracts.CommittedEvent;
using PbStreamEvent = Dolittle.Runtime.Events.Processing.Contracts.StreamEvent;

namespace Dolittle.SDK.Events.Processing.for_EventProcessingConverter
{
    public class when_converting_a_stream_event_to_sdk : given.a_converter
    {
        static bool partitioned;
        static Uuid partition_id;
        static Uuid scope_id;

        static PbCommittedEvent committed_event;
        static CommittedEvent committed_event_to_return;

        static PbStreamEvent stream_event;
        static StreamEvent converted_stream_event;

        Establish context = () =>
        {
            committed_event = new PbCommittedEvent();
            committed_event_to_return = new CommittedEvent(
                506549693,
                new DateTimeOffset(2020, 2, 20, 20, 20, 20, TimeSpan.Zero),
                "cc1326fc-ca2f-4ad4-a9fb-53ad9e771ee8",
                new ExecutionContext(
                    "388ed676-2448-45fe-99c7-93094e8241c4",
                    "6714619f-b2dd-4d90-a545-642df1bbb67b",
                    new Microservices.Version(5, 4, 3, 2, "duvorwuicu"),
                    "latmazokcu",
                    "a6d990c8-81cb-4a8b-b7cf-327c83ce7cc4",
                    new Claims(new[]
                    {
                        new Claim("bibetudicu", "efajeakuga", "wovawjohoa"),
                    }),
                    CultureInfo.InvariantCulture),
                new EventType("836faf72-6212-4351-adad-89bc1d7964c6", 3421559541),
                new object(),
                true);

            event_converter.Setup(_ => _.ToSDK(committed_event)).Returns(committed_event_to_return);

            partitioned = true;
            partition_id = Guid.Parse("8c0e8392-bec0-4fe1-bd7d-df2f555afe55").ToProtobuf();
            scope_id = Guid.Parse("2c87312c-c660-48db-a25a-a22622aeb4a0").ToProtobuf();

            stream_event = new PbStreamEvent
            {
                Event = committed_event,
                Partitioned = partitioned,
                PartitionId = partition_id,
                ScopeId = scope_id,
            };
        };

        Because of = () => converted_stream_event = event_processing_converter.ToSDK(stream_event);

        It should_call_the_event_converter_with_the_committed_event = () => event_converter.Verify(_ => _.ToSDK(committed_event));
        It should_return_the_result_from_the_event_converter = () => converted_stream_event.Event.ShouldBeTheSameAs(committed_event_to_return);
        It should_not_call_the_event_converter_for_anything_else = () => event_converter.VerifyNoOtherCalls();
        It should_have_the_correct_partitioned = () => converted_stream_event.Partitioned.ShouldEqual(partitioned);
        It should_have_the_correct_partition = () => converted_stream_event.Partition.ShouldEqual(partition_id.To<PartitionId>());
        It should_have_the_correct_scope = () => converted_stream_event.Scope.ShouldEqual(scope_id.To<ScopeId>());
    }
}