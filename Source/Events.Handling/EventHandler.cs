// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Diagnostics;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.SDK.Events.Handling.Builder.Methods;
using Dolittle.SDK.Execution;

namespace Dolittle.SDK.Events.Handling;

/// <summary>
/// An implementation of <see cref="IEventHandler" />.
/// </summary>
public class EventHandler : IEventHandler
{
    readonly IDictionary<EventType, IEventHandlerMethod> _eventHandlerMethods;
    readonly string _activityName;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventHandler"/> class.
    /// </summary>
    /// <param name="identifier">The <see cref="EventHandlerModelId" />.</param>
    /// <param name="eventHandlerMethods">The event handler methods by <see cref="EventType" />.</param>
    public EventHandler(
        EventHandlerModelId identifier,
        IDictionary<EventType, IEventHandlerMethod> eventHandlerMethods)
    {
        Identifier = identifier.Id;
        ScopeId = identifier.Scope;
        Partitioned = identifier.Partitioned;
        _eventHandlerMethods = eventHandlerMethods;
        if (!string.IsNullOrEmpty(identifier.Alias))
        {
            Alias = identifier.Alias;
        }
        _activityName = $"{(HasAlias ? Alias?.Value + "." : "")}Handle ";
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
    public EventHandlerAlias? Alias { get; }

    /// <inheritdoc />
    public bool HasAlias => Alias is not null; 

    /// <inheritdoc/>
    public async Task Handle(object @event, EventType eventType, EventContext context, IServiceProvider serviceProvider, CancellationToken cancellation)
    {
        using var activity = @event is not HandleEventRequest ? context.CommittedExecutionContext.StartChildActivity($"{_activityName}{@event.GetType().Name}")
            ?.Tag(eventType) : null;

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
