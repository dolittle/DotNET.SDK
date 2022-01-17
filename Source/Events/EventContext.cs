// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Concepts;
using Dolittle.SDK.Events.Store;
using Dolittle.SDK.Execution;

namespace Dolittle.SDK.Events;

/// <summary>
/// Represents the context in which an event occurred in.
/// </summary>
/// <param name="SequenceNumber">The <see cref="EventLogSequenceNumber">sequence number</see> that uniquely identifies the event in the event log which it was committed.</param>
/// <param name="EventType">The <see cref="EventType"/> of the event.</param>
/// <param name="EventSourceId">The <see cref="EventSourceId"/> that the event was committed to.</param>
/// <param name="Occurred">The <see cref="DateTimeOffset"/> when the event was committed to the <see cref="IEventStore"/>.</param>
/// <param name="CommittedExecutionContext">The <see cref="ExecutionContext"/> in which the event was committed to the <see cref="IEventStore"/>.</param>
/// <param name="CurrentExecutionContext">The <see cref="ExecutionContext"/> in which the event is currently being processed.</param>
public record EventContext(
    EventLogSequenceNumber SequenceNumber,
    EventType EventType,
    EventSourceId EventSourceId,
    DateTimeOffset Occurred,
    ExecutionContext CommittedExecutionContext,
    ExecutionContext CurrentExecutionContext);
