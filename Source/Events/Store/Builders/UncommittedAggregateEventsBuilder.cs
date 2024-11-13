// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.SDK.Events.Builders;

namespace Dolittle.SDK.Events.Store.Builders;

/// <summary>
/// Represents a builder for <see cref="UncommittedAggregateEvents" />.
/// </summary>
public class UncommittedAggregateEventsBuilder
{
    readonly EventSourceId _eventSourceId;
    readonly IList<EventBuilder> _builders = [];
    readonly AggregateRootId _aggregateRootId;
    readonly AggregateRootVersion _expectedVersion;

    /// <summary>
    /// Initializes a new instance of the <see cref="UncommittedAggregateEventsBuilder"/> class.
    /// </summary>
    /// <param name="aggregateRootId">The <see cref="AggregateRootId" />.</param>
    /// <param name="eventSourceId">The <see cref="EventSourceId" />.</param>
    /// <param name="expectedVersion">The <see cref="AggregateRootVersion" />.</param>
    public UncommittedAggregateEventsBuilder(AggregateRootId aggregateRootId, EventSourceId eventSourceId, AggregateRootVersion expectedVersion)
    {
        _eventSourceId = eventSourceId;
        _aggregateRootId = aggregateRootId;
        _expectedVersion = expectedVersion;
    }

    /// <summary>
    /// Create an aggregate event.
    /// </summary>
    /// <param name="content">The event content.</param>
    /// <returns>The <see cref="EventBuilder" /> for continuation.</returns>
    public EventBuilder CreateEvent(object content) => CreateEvent(content, false);

    /// <summary>
    /// Create a public aggregate event.
    /// </summary>
    /// <param name="content">The event content.</param>
    /// <returns>The <see cref="EventBuilder" /> for continuation.</returns>
    public EventBuilder CreatePublicEvent(object content) => CreateEvent(content, true);

    /// <summary>
    /// Build uncommitted aggregate event.
    /// </summary>
    /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
    /// <returns>The <see cref="UncommittedAggregateEvent" />.</returns>
    public UncommittedAggregateEvents Build(IEventTypes eventTypes)
    {
        var events = new UncommittedAggregateEvents(_eventSourceId, _aggregateRootId, _expectedVersion);
        foreach (var builder in _builders)
        {
            var (content, eventType, _, isPublic) = builder.Build(eventTypes);
            events.Add(new UncommittedAggregateEvent(eventType, content, isPublic));
        }

        return events;
    }

    EventBuilder CreateEvent(object content, bool isPublic)
    {
        var builder = new EventBuilder(content, _eventSourceId, isPublic);
        _builders.Add(builder);
        return builder;
    }
}
