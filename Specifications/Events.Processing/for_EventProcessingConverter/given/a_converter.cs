// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Dolittle.SDK.Events.Store;
using Dolittle.SDK.Events.Store.Converters;
using Machine.Specifications;
using Moq;
using It = Moq.It;
using PbCommittedEvent = Dolittle.Runtime.Events.Contracts.CommittedEvent;

namespace Dolittle.SDK.Events.Processing.for_EventProcessingConverter.given;

delegate void TryConvert(PbCommittedEvent source, out CommittedEvent @event, [NotNullWhen(false)] out Exception error);

public class a_converter
{
    protected static Mock<IConvertEventsToSDK> event_converter;
    protected static List<PbCommittedEvent> converted_events;

    protected static EventProcessingConverter event_processing_converter;

    Establish context = () =>
    {
        event_converter = new Mock<IConvertEventsToSDK>();
        converted_events = new List<PbCommittedEvent>();

        event_processing_converter = new EventProcessingConverter(event_converter.Object);
    };

    protected static void SetupConverterToReturn(PbCommittedEvent protobufEvent, CommittedEvent sdkEvent)
    {
        event_converter
            .Setup(_ => _.TryConvert(
                protobufEvent,
                out It.Ref<CommittedEvent>.IsAny,
                out It.Ref<Exception>.IsAny))
            .Callback(new TryConvert((
                PbCommittedEvent source,
                out CommittedEvent @event,
                [NotNullWhen(false)] out Exception error) =>
            {
                converted_events.Add(source);
                @event = sdkEvent;
                error = null;
            }))
            .Returns(true);
    }
}