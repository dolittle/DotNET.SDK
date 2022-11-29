// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Common.Model;

namespace Dolittle.SDK.Events.Handling;

/// <summary>
/// Decorates a class to indicate the Event Handler Id of the Event Handler class.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class EventHandlerAttribute : Attribute, IDecoratedTypeDecorator<EventHandlerModelId>
{
    readonly EventHandlerId _eventHandlerId;
    readonly string? _alias;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventHandlerAttribute"/> class.
    /// </summary>
    /// <param name="eventHandlerId">The unique identifier of the event handler.</param>
    /// <param name="partitioned">Whether the event handler is partitioned.</param>
    /// <param name="inScope">The scope that the event handler handles events in.</param>
    /// <param name="alias">The alias for the event handler.</param>
    public EventHandlerAttribute(string eventHandlerId, bool partitioned = true, string? inScope = null, string? alias = null)
    {
        _eventHandlerId = eventHandlerId;
        _alias = alias;
        Partitioned = partitioned;
        Scope = inScope ?? ScopeId.Default;
    }

    /// <summary>
    /// Gets a value indicating whether this event handler is partitioned.
    /// </summary>
    public bool Partitioned { get; }

    /// <summary>
    /// Gets the <see cref="ScopeId" />.
    /// </summary>
    public ScopeId Scope { get; }


    /// <inheritdoc />
    public EventHandlerModelId GetIdentifier(Type decoratedType) => new(_eventHandlerId, Partitioned, Scope, _alias ?? decoratedType.Name);
}
