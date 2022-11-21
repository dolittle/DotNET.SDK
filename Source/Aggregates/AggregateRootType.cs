// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Artifacts;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Aggregates;

/// <summary>
/// Represents the type of an aggregate root.
/// </summary>
public class AggregateRootType : Artifact<AggregateRootId>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateRootType"/> class.
    /// </summary>
    /// <param name="id">The <see cref="AggregateRootId">unique identifier</see> of the <see cref="AggregateRootType"/>.</param>
    public AggregateRootType(AggregateRootId id)
        : this(id, Generation.First, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateRootType"/> class.
    /// </summary>
    /// <param name="id">The <see cref="AggregateRootId">unique identifier</see> of the <see cref="AggregateRootType"/>.</param>
    /// <param name="alias"><see cref="AggregateRootAlias">Alias</see> of the <see cref="AggregateRootType"/>.</param>
    public AggregateRootType(AggregateRootId id, AggregateRootAlias alias)
        : this(id, Generation.First, alias)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateRootType"/> class.
    /// </summary>
    /// <param name="id">The <see cref="AggregateRootId">unique identifier</see> of the <see cref="AggregateRootType"/>.</param>
    /// <param name="generation"><see cref="Generation">Generation</see> of the <see cref="AggregateRootType"/>.</param>
    public AggregateRootType(AggregateRootId id, Generation generation)
        : this(id, generation, null)
    {
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateRootType"/> class.
    /// </summary>
    /// <param name="id">The <see cref="AggregateRootId">unique identifier</see> of the <see cref="AggregateRootType"/>.</param>
    /// <param name="generation"><see cref="Generation">Generation</see> of the <see cref="AggregateRootType"/>.</param>
    /// <param name="alias"><see cref="AggregateRootAlias">Alias</see> of the <see cref="AggregateRootType"/>.</param>
    public AggregateRootType(AggregateRootId id, Generation generation, AggregateRootAlias alias)
        : base(id, generation, alias)
    {
        ThrowIfAggregateRootTypeIdIsNull(id);
        ThrowIfGenerationIsNull(generation);
        Alias = alias;
    }

    /// <summary>
    /// Gets the alias for the Aggregate Root.
    /// </summary>
    public AggregateRootAlias? Alias { get; }

    /// <summary>
    /// Gets a value indicating whether the Aggregate Root has an alias or not.
    /// </summary>
    public bool HasAlias => !string.IsNullOrEmpty(Alias?.Value);

    static void ThrowIfAggregateRootTypeIdIsNull(AggregateRootId id)
    {
        if (id == null)
        {
            throw new AggregateRootIdCannotBeNull();
        }
    }

    static void ThrowIfGenerationIsNull(Generation generation)
    {
        if (generation == null)
        {
            throw new AggregateRootTypeGenerationCannotBeNull();
        }
    }
}
