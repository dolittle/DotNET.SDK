// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.SDK.Artifacts;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Aggregates;

/// <summary>
/// Represents an implementation of <see cref="IAggregateRootTypes" />.
/// </summary>
public class AggregateRootTypes : Artifacts<AggregateRootType, AggregateRootId>, IAggregateRootTypes
{
    /// <summary>
    /// Initializes an instance of the <see cref="AggregateRootTypes"/> class.
    /// </summary>
    /// <param name="associations">The <see cref="AggregateRootType"/> associations.</param>
    public AggregateRootTypes(IDictionary<Type, AggregateRootType> associations)
        : base(associations)
    {
    }

    /// <inheritdoc/>
    protected override Exception CreateNoArtifactAssociatedWithType(Type type)
        => new AggregateRootTypeAssociatedWithType(type);

    /// <inheritdoc/>
    protected override Exception CreateNoTypeAssociatedWithArtifact(AggregateRootType artifact)
        => new NoTypeAssociatedWithAggregateRootType(artifact);
}
