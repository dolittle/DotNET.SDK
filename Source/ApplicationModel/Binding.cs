// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Concepts;

namespace Dolittle.SDK.ApplicationModel;

/// <summary>
/// Represents an instance of <see cref="IBinding{TIdentifier,TId}"/>.
/// </summary>
/// <typeparam name="TIdentifier">The <see cref="Type"/> of the <see cref="IIdentifier{TId}"/>.</typeparam>
/// <typeparam name="TId">The type of the globally unique id of the identifier.</typeparam>
public record Binding<TIdentifier, TId> : Binding, IBinding<TIdentifier, TId>
    where TIdentifier : IIdentifier<TId>
    where TId : ConceptAs<Guid>
{
    /// <summary>
    /// Initializes an instance of the <see cref="Binding{TIdentifier,TId}"/> class.
    /// </summary>
    /// <param name="identifier">The <typeparamref name="TIdentifier"/>.</param>
    public Binding(TIdentifier identifier) : base(identifier)
    {
        Identifier = identifier;
    }

    /// <inheritdoc />
    public new TIdentifier Identifier { get; }
}

/// <summary>
/// Represents an instance of <see cref="IBinding"/>.
/// </summary>
/// <param name="Identifier">The <see cref="IIdentifier"/> that is bound to something.</param>
public record Binding(IIdentifier Identifier) : IBinding;
