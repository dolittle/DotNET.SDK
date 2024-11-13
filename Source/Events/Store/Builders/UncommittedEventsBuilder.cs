// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Dolittle.SDK.Events.Store.Builders;

/// <summary>
/// Represents a builder for <see cref="UncommittedEvents" />.
/// </summary>
public class UncommittedEventsBuilder
{
    readonly IList<UncommittedEventBuilder> _builders = [];

    /// <summary>
    /// Build an event.
    /// </summary>
    /// <param name="event">The event.</param>
    /// <returns>The <see cref="UncommittedEventBuilder" /> for continuation.</returns>
    public UncommittedEventBuilder CreateEvent(object @event)
        => CreateBuilder(@event, false);

    /// <summary>
    /// Build a public event.
    /// </summary>
    /// <param name="event">The event.</param>
    /// <returns>The <see cref="UncommittedEventBuilder" /> for continuation.</returns>
    public UncommittedEventBuilder CreatePublicEvent(object @event)
        => CreateBuilder(@event, true);

    /// <summary>
    /// Builds the uncommitted events.
    /// </summary>
    /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
    /// <returns>The <see cref="UncommittedEvents" />.</returns>
    public UncommittedEvents Build(IEventTypes eventTypes)
    {
        var events = new UncommittedEvents();
        foreach (var builder in _builders) events.Add(builder.Build(eventTypes));
        return events;
    }

    UncommittedEventBuilder CreateBuilder(object @event, bool isPublic)
    {
        var builder = new UncommittedEventBuilder(@event, isPublic);
        _builders.Add(builder);
        return builder;
    }
}
