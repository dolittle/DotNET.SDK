// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.ApplicationModel;
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
    /// <param name="generation"><see cref="Generation">Generation</see> of the <see cref="AggregateRootType"/>.</param>
    /// <param name="alias"><see cref="IdentifierAlias">Alias</see> of the <see cref="AggregateRootType"/>.</param>
    public AggregateRootType(AggregateRootId id, Generation? generation = null, IdentifierAlias? alias = null)
        : base(id, generation, alias)
    {
    }
}
