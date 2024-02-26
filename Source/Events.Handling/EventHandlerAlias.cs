// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Concepts;

namespace Dolittle.SDK.Events.Handling;

/// <summary>
/// Represents the concept of an alias for an Event Handler.
/// </summary>
public record EventHandlerAlias(string Value) : ConceptAs<string>(Value)
{
    /// <summary>
    /// Implicitly converts from a <see cref="string"/> to an <see cref="EventHandlerAlias"/>.
    /// </summary>
    /// <param name="alias">The <see cref="string"/> representation.</param>
    /// <returns>The converted <see cref="EventHandlerAlias"/>.</returns>
    public static implicit operator EventHandlerAlias(string alias) => new(alias);
}
