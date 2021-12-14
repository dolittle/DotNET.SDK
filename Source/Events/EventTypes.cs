// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.SDK.Artifacts;

namespace Dolittle.SDK.Events;

/// <summary>
/// Represents an implementation of <see cref="IEventTypes" />.
/// </summary>
public class EventTypes : Artifacts<EventType, EventTypeId>, IEventTypes
{
    /// <summary>
    /// Initializes an instance of the <see cref="EventTypes"/> class.
    /// </summary>
    /// <param name="associations">The <see cref="EventType"/> associations.</param>
    public EventTypes(IDictionary<Type, EventType> associations) : base(associations)
    {
    }

    /// <inheritdoc/>
    protected override Exception CreateNoArtifactAssociatedWithType(Type type)
        => new NoEventTypeAssociatedWithType(type);

    /// <inheritdoc/>
    protected override Exception CreateNoTypeAssociatedWithArtifact(EventType artifact)
        => new NoTypeAssociatedWithEventType(artifact);
}
