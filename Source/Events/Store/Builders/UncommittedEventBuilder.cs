// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events.Builders;

namespace Dolittle.SDK.Events.Store.Builders;

/// <summary>
/// Represents a builder for <see cref="UncommittedEvent" />.
/// </summary>
public class UncommittedEventBuilder
{
    readonly object _event;
    readonly bool _isPublic;
    EventBuilder _builder;

    /// <summary>
    /// Initializes a new instance of the <see cref="UncommittedEventBuilder"/> class.
    /// </summary>
    /// <param name="event">The event content.</param>
    /// <param name="isPublic">Whether the event is public.</param>
    public UncommittedEventBuilder(object @event, bool isPublic)
    {
        _event = @event;
        _isPublic = isPublic;
    }

    /// <summary>
    /// Builds an uncommitted event from an event source.
    /// </summary>
    /// <param name="eventSourceId">The <see cref="EventSourceId" />.</param>
    /// <returns>The <see cref="EventBuilder" /> for continuation.</returns>
    public EventBuilder FromEventSource(EventSourceId eventSourceId)
    {
        ThrowIfEventSourceIsAlreadyDefined();
        _builder = new EventBuilder(_event, eventSourceId, _isPublic);
        return _builder;
    }

    /// <summary>
    /// Build an uncommitted event.
    /// </summary>
    /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
    /// <returns>The <see cref="UncommittedEvent" />.</returns>
    public UncommittedEvent Build(IEventTypes eventTypes)
    {
        ThrowIfEventSourceIsNotDefined();
        var (content, eventType, eventSourceId, isPublic) = _builder.Build(eventTypes);
        return new UncommittedEvent(eventSourceId, eventType, content, isPublic);
    }

    void ThrowIfEventSourceIsAlreadyDefined()
    {
        if (_builder != default)
        {
            throw new EventBuilderMethodAlreadyCalled("FromEventSource()");
        }
    }

    void ThrowIfEventSourceIsNotDefined()
    {
        if (_builder == default)
        {
            throw new EventDefinitionIncomplete("EventSource", "Call FromEventSource() with the event source id");
        }
    }
}
