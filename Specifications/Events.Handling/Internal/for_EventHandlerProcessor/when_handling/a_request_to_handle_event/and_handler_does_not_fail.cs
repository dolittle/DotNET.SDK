// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.SDK.Events.Store;
using Dolittle.SDK.Protobuf;
using Machine.Specifications;

namespace Dolittle.SDK.Events.Handling.Internal.for_EventHandlerProcessor.when_handling.a_request_to_handle_event;

public class and_handler_does_not_fail : given.all_dependencies
{
    static EventContext event_context;
    static EventHandlerResponse response;

    Establish context = () =>
    {
        var stream_event = new Processing.StreamEvent(
            new CommittedEvent(
                committed_event.EventLogSequenceNumber,
                committed_event.Occurred.ToDateTimeOffset(),
                committed_event.EventSourceId,
                execution_context,
                event_type_to_handle,
                event_to_handle,
                committed_event.Public),
            partitioned,
            request.Event.PartitionId,
            request.Event.ScopeId.To<ScopeId>());
        event_processing_converter
            .Setup(_ => _.ToSDK(request.Event))
            .Returns(stream_event);
        event_context = new EventContext(
            committed_event.EventLogSequenceNumber,
            committed_event.EventType.To<EventType, EventTypeId>(),
            committed_event.EventSourceId,
            committed_event.Occurred.ToDateTimeOffset(),
            execution_context,
            execution_context);
        event_types.Setup(_ => _.HasTypeFor(event_type_to_handle)).Returns(true);
        event_types.Setup(_ => _.GetTypeFor(event_type_to_handle)).Returns(typeof(given.some_event));
        event_handler
            .Setup(_ => _.Handle(Moq.It.IsAny<object>(), Moq.It.IsAny<EventType>(), Moq.It.IsAny<EventContext>(), Moq.It.IsAny<IServiceProvider>(), Moq.It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        event_handler.Invocations.Clear();
    };

    Because of = () => response = event_handler_processor.Handle(request, execution_context, service_provider.Object, cancellation_token).GetAwaiter().GetResult();

    It should_get_a_response = () => response.ShouldNotBeNull();
    It should_not_have_a_failure = () => response.Failure.ShouldBeNull();
    It should_have_called_the_handler_once_to_handle_the_event = () => event_handler.Verify(_ => _.Handle(event_to_handle, event_type_to_handle, event_context, service_provider.Object, cancellation_token), Moq.Times.Once);
    It should_only_have_called_the_handler_to_handle_the_event = () => event_handler.VerifyNoOtherCalls();
}