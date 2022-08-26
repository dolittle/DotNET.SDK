// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.ApplicationModel;

namespace Dolittle.SDK.Events.Handling;

/// <summary>
/// Represents the identifier of an event handler in an application model.
/// </summary>
public class EventHandlerModelId : Identifier<EventHandlerId, ScopeId>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EventHandlerModelId"/> class.
    /// </summary>
    /// <param name="id">The <see cref="EventHandlerId"/>.</param>
    /// <param name="partitioned">The value indicating whether the event handler is partitioned.</param>
    /// <param name="scope">The <see cref="ScopeId"/>.></param>
    /// <param name="alias">The alias.</param>
    public EventHandlerModelId(EventHandlerId id, bool partitioned, ScopeId scope, IdentifierAlias? alias)
        : base("EventHandler", id, scope, alias)
    {
        Partitioned = partitioned;
        Scope = scope;
    }
    
    /// <summary>
    /// Gets whether the event handler is partitioned.
    /// </summary>
    public bool Partitioned { get; }
    
    /// <summary>
    /// Gets the <see cref="ScopeId"/>.
    /// </summary>
    public ScopeId Scope { get; }
}
