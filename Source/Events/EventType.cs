// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.ApplicationModel;
using Dolittle.SDK.Artifacts;

namespace Dolittle.SDK.Events;

/// <summary>
/// Represents the type of an event.
/// </summary>
public class EventType : Artifact<EventTypeId>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EventType"/> class.
    /// </summary>
    /// <param name="id">The <see cref="EventTypeId">unique identifier</see> of the <see cref="EventType"/>.</param>
    /// <param name="generation"><see cref="Generation">Generation</see> of the <see cref="EventType"/>.</param>
    /// <param name="alias"><see cref="IdentifierAlias">Alias</see> of the <see cref="EventType"/>.</param>
    public EventType(EventTypeId id, Generation? generation = null, IdentifierAlias? alias = null)
        : base(id, generation, alias)
    {
    }
    
}
