// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Events.Store.Builders;

namespace Dolittle.SDK.Events.Store;

/// <summary>
/// Represents an implementation of <see cref="ICommitEvents" />.
/// </summary>
public class EventCommitter : ICommitEvents
{
    readonly Internal.ICommitEvents _events;
    readonly IEventTypes _eventTypes;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventCommitter"/> class.
    /// </summary>
    /// <param name="events">The <see cref="Internal.ICommitEvents" />.</param>
    /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
    public EventCommitter(Internal.ICommitEvents events, IEventTypes eventTypes)
    {
        _events = events;
        _eventTypes = eventTypes;
    }

    /// <inheritdoc/>
    public Task<CommittedEvents> CommitEvent(
        object content,
        EventSourceId eventSourceId,
        CancellationToken cancellationToken = default)
        => Commit(builder => BuildEvent(builder, content, eventSourceId), cancellationToken);

    /// <inheritdoc/>
    public Task<CommittedEvents> CommitPublicEvent(
        object content,
        EventSourceId eventSourceId,
        CancellationToken cancellationToken = default)
        => Commit(builder => BuildEvent(builder, content, eventSourceId, isPublic: true), cancellationToken);

    /// <inheritdoc/>
    public Task<CommittedEvents> Commit(
        Action<UncommittedEventsBuilder> callback,
        CancellationToken cancellationToken = default)
    {
        var builder = new UncommittedEventsBuilder();
        callback(builder);
        return _events.Commit(builder.Build(_eventTypes));
    }

    void BuildEvent(
        UncommittedEventsBuilder builder,
        object content,
        EventSourceId eventSourceId,
        bool isPublic = false,
        EventType eventType = default)
    {
        var uncommittedEventBuilder = isPublic ? builder.CreatePublicEvent(content) : builder.CreateEvent(content);
        var eventBuilder = uncommittedEventBuilder.FromEventSource(eventSourceId);
        if (eventType != default)
        {
            eventBuilder.WithEventType(eventType);
        }
    }
}
