// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Concepts;

namespace Dolittle.SDK.Common.Model;

/// <summary>
/// Represents a binding for a specific type of identifier to a type.
/// </summary>
/// <param name="Identifier">The identifier that is bound.</param>
/// <param name="Type">The <see cref="Type"/> that the identifier is bound to.</param>
/// <typeparam name="TIdentifier">The <see cref="Type"/> of the <see cref="IIdentifier{TId}"/>.</typeparam>
/// <typeparam name="TId">The type of the globally unique id of the identifier.</typeparam>
public record TypeBinding<TIdentifier, TId>(TIdentifier Identifier, Type Type) : Binding<TIdentifier, TId>(Identifier)
    where TIdentifier : IIdentifier<TId>
    where TId : ConceptAs<Guid>
{
    /// <inheritdoc />
    public override string ToString()
        => $"type binding from {Identifier} to type {Type}";
}
