// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Diagnostics;
using Dolittle.SDK.Events.Handling.Builder.Methods;
using Dolittle.SDK.Execution;
using OpenTelemetry.Trace;

namespace Dolittle.SDK.Events.Handling;

/// <summary>
/// An implementation of <see cref="IEventHandler" />.
/// </summary>
public class EventHandler : IEventHandler
{
    readonly IDictionary<EventType, IEventHandlerMethod> _eventHandlerMethods;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventHandler"/> class.
    /// </summary>
    /// <param name="identifier">The <see cref="EventHandlerId" />.</param>
    /// <param name="scopeId">The <see cref="ScopeId" />.</param>
    /// <param name="partitioned">The value indicating whether the <see cref="EventHandler" /> is partitioned.</param>
    /// <param name="eventHandlerMethods">The event handler methods by <see cref="EventType" />.</param>
    public EventHandler(
        EventHandlerId identifier,
        ScopeId scopeId,
        bool partitioned,
        IDictionary<EventType, IEventHandlerMethod> eventHandlerMethods)
    {
        Identifier = identifier;
        ScopeId = scopeId;
        Partitioned = partitioned;
        _eventHandlerMethods = eventHandlerMethods;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventHandler"/> class.
    /// </summary>
    /// <param name="identifier">The <see cref="EventHandlerId" />.</param>
    /// <param name="alias">The <see cref="EventHandlerAlias"/>.</param>
    /// <param name="scopeId">The <see cref="ScopeId" />.</param>
    /// <param name="partitioned">The value indicating whether the <see cref="EventHandler" /> is partitioned.</param>
    /// <param name="eventHandlerMethods">The event handler methods by <see cref="EventType" />.</param>
    public EventHandler(
        EventHandlerId identifier,
        EventHandlerAlias alias,
        ScopeId scopeId,
        bool partitioned,
        IDictionary<EventType, IEventHandlerMethod> eventHandlerMethods)
        : this(identifier, scopeId, partitioned, eventHandlerMethods)
    {
        Alias = alias;
        HasAlias = true;
    }

    /// <inheritdoc/>
    public EventHandlerId Identifier { get; }

    /// <inheritdoc/>
    public ScopeId ScopeId { get; }

    /// <inheritdoc/>
    public bool Partitioned { get; }

    /// <inheritdoc/>
    public IEnumerable<EventType> HandledEvents => _eventHandlerMethods.Keys;

    /// <inheritdoc />
    public EventHandlerAlias Alias { get; }

    /// <inheritdoc />
    public bool HasAlias { get; }

    /// <inheritdoc/>
    public async Task Handle(object @event, EventType eventType, EventContext context, IServiceProvider serviceProvider, CancellationToken cancellation)
    {
        using var activity = context.CommittedExecutionContext.StartChildActivity($"{(HasAlias ? Alias.Value + "." : "")}Handle {@event.GetType().Name}")
            ?.Tag(eventType);

        try
        {
            if (!_eventHandlerMethods.TryGetValue(eventType, out var method))
            {
                throw new MissingEventHandlerForEventType(eventType);
            }

            Exception exception = await method.TryHandle(@event, context, serviceProvider).ConfigureAwait(false);
            if (exception != default)
            {
                throw new EventHandlerMethodFailed(Identifier, eventType, @event, exception);
            }
        }
        catch (Exception e)
        {
            activity?.RecordError(e);
            throw;
        }

        activity?.SetStatus(ActivityStatusCode.Ok);
    }
}
