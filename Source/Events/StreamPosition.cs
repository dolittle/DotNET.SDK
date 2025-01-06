// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Concepts;

namespace Dolittle.SDK.Events;

/// <summary>
/// Represents the position of the event in a stream. 0-indexed.
/// </summary>
public record StreamPosition(ulong Value) : ConceptAs<ulong>(Value)
{
    /// <summary>
    /// Implicitly convert a <see cref="uint"/> to an <see cref="StreamPosition"/>.
    /// </summary>
    /// <param name="number">The number.</param>
    public static implicit operator StreamPosition(ulong number) => new(number);
}
