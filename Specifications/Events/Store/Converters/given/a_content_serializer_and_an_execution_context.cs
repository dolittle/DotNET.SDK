// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.Execution.Contracts;
using Dolittle.SDK.Protobuf;
using Dolittle.Security.Contracts;
using Machine.Specifications;
using Moq;
using It = Moq.It;
using Version = Dolittle.Versioning.Contracts.Version;

namespace Dolittle.SDK.Events.Store.Converters.given
{
    delegate void TrySerialize(
        object content,
        out string json,
        out Exception error);

    delegate void TryDeserialize(
        EventType eventType,
        EventLogSequenceNumber sequenceNumber,
        string source,
        out object content,
        out Exception error);

    public class a_content_serializer_and_an_execution_context
    {
        protected static Mock<ISerializeEventContent> serializer;

        protected static List<EventType> deserialized_event_types;
        protected static List<EventLogSequenceNumber> deserialized_sequence_numbers;
        protected static List<string> deserialized_contents;

        protected static List<object> serialized_contents;

        protected static ExecutionContext execution_context;

        Establish context = () =>
        {
            serializer = new Mock<ISerializeEventContent>();

            deserialized_event_types = new List<EventType>();
            deserialized_sequence_numbers = new List<EventLogSequenceNumber>();
            deserialized_contents = new List<string>();

            serialized_contents = new List<object>();

            execution_context = new ExecutionContext
            {
                TenantId = Guid.Parse("a3f2c429-9365-4e3f-935b-e43af7f5cadc").ToProtobuf(),
                CorrelationId = Guid.Parse("c2f187b1-f665-4d43-97a0-ec0196de78dd").ToProtobuf(),
                MicroserviceId = Guid.Parse("6b31e2eb-f792-4f71-bc7d-99ba02fafa21").ToProtobuf(),
                Environment = "giwhiwarof",
                Version = new Version
                {
                    Major = 13,
                    Minor = 7,
                    Patch = 5,
                    Build = 1001,
                    PreReleaseString = "tahpigheze",
                },
            };
            execution_context.Claims.Add(new Claim
                {
                    Key = "dijjufpubu",
                    Value = "heletfokha",
                    ValueType = "comisihsoc"
                });
            execution_context.Claims.Add(new Claim
                {
                    Key = "taudpusiwadapek",
                    Value = "nalfa",
                    ValueType = "azjetmunigpuavwomuze"
                });
        };

        protected static void SetupDeserializeToReturnObject(string source, object toReturn)
        {
            serializer
                .Setup(_ => _.TryDeserialize(
                    It.IsAny<EventType>(),
                    It.IsAny<EventLogSequenceNumber>(),
                    source,
                    out It.Ref<object>.IsAny,
                    out It.Ref<Exception>.IsAny))
                .Callback(new TryDeserialize((
                    EventType eventType,
                    EventLogSequenceNumber sequenceNumber,
                    string source,
                    out object content,
                    out Exception error) =>
                    {
                        deserialized_event_types.Add(eventType);
                        deserialized_sequence_numbers.Add(sequenceNumber);
                        deserialized_contents.Add(source);
                        content = toReturn;
                        error = null;
                    }))
                .Returns(true);
        }

        protected static void SetupDeserializeToFail(string source, Exception toReturn)
        {
            serializer
                .Setup(_ => _.TryDeserialize(
                    It.IsAny<EventType>(),
                    It.IsAny<EventLogSequenceNumber>(),
                    source,
                    out It.Ref<object>.IsAny,
                    out It.Ref<Exception>.IsAny))
                .Callback(new TryDeserialize((
                    EventType eventType,
                    EventLogSequenceNumber sequenceNumber,
                    string source,
                    out object content,
                    out Exception error) =>
                    {
                        deserialized_event_types.Add(eventType);
                        deserialized_sequence_numbers.Add(sequenceNumber);
                        deserialized_contents.Add(source);
                        content = new object();
                        error = toReturn;
                    }))
                .Returns(false);
        }

        protected static void SetupSerializeToReturnJSON(object source, string toReturn)
        {
            serializer
                .Setup(_ => _.TrySerialize(
                    source,
                    out It.Ref<string>.IsAny,
                    out It.Ref<Exception>.IsAny))
                .Callback(new TrySerialize((
                    object content,
                    out string json,
                    out Exception error) =>
                    {
                        serialized_contents.Add(content);
                        json = toReturn;
                        error = null;
                    }))
                .Returns(true);
        }

        protected static void SetupSerializeToFail(object source, Exception toReturn)
        {
            serializer
                .Setup(_ => _.TrySerialize(
                    source,
                    out It.Ref<string>.IsAny,
                    out It.Ref<Exception>.IsAny))
                .Callback(new TrySerialize((
                    object content,
                    out string json,
                    out Exception error) =>
                    {
                        serialized_contents.Add(content);
                        json = "";
                        error = toReturn;
                    }))
                .Returns(false);
        }
    }
}
