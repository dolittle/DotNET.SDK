// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Artifacts;
using Dolittle.SDK.Common.Model;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Aggregates;

/// <summary>
/// The attribute for deciding the <see cref="AggregateRootId" /> of an <see cref="AggregateRoot" />.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class AggregateRootAttribute : Attribute, IDecoratedTypeDecorator<AggregateRootType>
{
    readonly AggregateRootId _id;
    readonly string? _alias;
    readonly Generation _generation = Generation.First;

    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateRootAttribute"/> class.
    /// </summary>
    /// <param name="id">The unique identifier.</param>
    /// <param name="alias">The alias for the aggregate root.</param>
    public AggregateRootAttribute(string id, string? alias = null)
    {
        _id = id;
        _alias = alias;
    }

    /// <inheritdoc />
    public AggregateRootType GetIdentifier(Type decoratedType) => new(_id, _generation, _alias ?? decoratedType.Name);
}
