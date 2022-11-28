// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Common.Model;

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
    /// <param name="scope">The <see cref="ScopeId"/>.></param>
    public EventHandlerModelId(EventHandlerId id, ScopeId scope)
        : base("EventHandler", id, scope)
    {
        Scope = scope;
    }
    
    /// <summary>
    /// Gets the <see cref="ScopeId"/>.
    /// </summary>
    public ScopeId Scope { get; }
}
