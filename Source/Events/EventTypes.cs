// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Artifacts;

namespace Dolittle.SDK.Events;

/// <summary>
/// Represents an implementation of <see cref="IEventTypes" />.
/// </summary>
public class EventTypes : Artifacts<EventType, EventTypeId>, IEventTypes
{
    /// <inheritdoc/>
    protected override Exception CreateNoArtifactAssociatedWithType(Type type)
        => new NoEventTypeAssociatedWithType(type);

    /// <inheritdoc/>
    protected override Exception CreateNoTypeAssociatedWithArtifact(EventType artifact)
        => new NoTypeAssociatedWithEventType(artifact);

    /// <inheritdoc/>
    protected override Exception CreateCannotAssociateMultipleArtifactsWithType(Type type, EventType artifact, EventType existing)
        => new CannotAssociateMultipleEventTypesWithType(type, artifact, existing);

    /// <inheritdoc/>
    protected override Exception CreateCannotAssociateMultipleTypesWithArtifact(EventType artifact, Type type, Type existing)
        => new CannotAssociateMultipleTypesWithEventType(artifact, type, existing);
}
