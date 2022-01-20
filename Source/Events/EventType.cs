// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Artifacts;

namespace Dolittle.SDK.Events;

/// <summary>
/// Represents the type of an event.
/// </summary>
public record EventType : Artifact<EventTypeId>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EventType"/> class.
    /// </summary>
    /// <param name="id">The <see cref="EventTypeId">unique identifier</see> of the <see cref="EventType"/>.</param>
    public EventType(EventTypeId id)
        : base(id)
    {
        ThrowIfEventTypeIdIsNull(id);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventType"/> class.
    /// </summary>
    /// <param name="id">The <see cref="EventTypeId">unique identifier</see> of the <see cref="EventType"/>.</param>
    /// <param name="alias"><see cref="EventTypeAlias">Alias</see> of the <see cref="EventType"/>.</param>
    public EventType(EventTypeId id, EventTypeAlias alias)
        : base(id)
    {
        ThrowIfEventTypeIdIsNull(id);
        Alias = alias;
        HasAlias = true;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventType"/> class.
    /// </summary>
    /// <param name="id">The <see cref="EventTypeId">unique identifier</see> of the <see cref="EventType"/>.</param>
    /// <param name="generation"><see cref="Generation">Generation</see> of the <see cref="EventType"/>.</param>
    public EventType(EventTypeId id, Generation generation)
        : base(id, generation)
    {
        ThrowIfEventTypeIdIsNull(id);
        ThrowIfGenerationIsNull(generation);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventType"/> class.
    /// </summary>
    /// <param name="id">The <see cref="EventTypeId">unique identifier</see> of the <see cref="EventType"/>.</param>
    /// <param name="generation"><see cref="Generation">Generation</see> of the <see cref="EventType"/>.</param>
    /// <param name="alias"><see cref="EventTypeAlias">Alias</see> of the <see cref="EventType"/>.</param>
    public EventType(EventTypeId id, Generation generation, EventTypeAlias alias)
        : base(id, generation)
    {
        ThrowIfEventTypeIdIsNull(id);
        ThrowIfGenerationIsNull(generation);
        Alias = alias;
        HasAlias = true;
    }

    /// <summary>
    /// Gets the alias for the Event Type.
    /// </summary>
    public EventTypeAlias Alias { get; }

    /// <summary>
    /// Gets a value indicating whether the Event Type has an alias or not.
    /// </summary>
    public bool HasAlias { get; }

    static void ThrowIfEventTypeIdIsNull(EventTypeId id)
    {
        if (id == null)
        {
            throw new EventTypeIdCannotBeNull();
        }
    }

    static void ThrowIfGenerationIsNull(Generation generation)
    {
        if (generation == null)
        {
            throw new EventTypeGenerationCannotBeNull();
        }
    }
}
