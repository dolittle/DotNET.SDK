// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Concepts;

namespace Dolittle.SDK.Aggregates;

/// <summary>
/// Represents the alias of an event type.
/// </summary>
public record AggregateRootAlias(string Value) : ConceptAs<string>(Value)
{
    /// <summary>
    /// Implicitly convert from a <see cref="string"/> to an <see cref="AggregateRootAlias"/>.
    /// </summary>
    /// <param name="alias">AggregateRootAlias as <see cref="string"/>.</param>
    public static implicit operator AggregateRootAlias(string alias) => new(alias);
}
