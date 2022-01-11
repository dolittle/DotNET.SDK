// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Concepts;

namespace Dolittle.SDK.Common.Model;

/// <summary>
/// Represents a binding between an <see cref="IIdentifier"/> and something.
/// </summary>
public interface IBinding
{
    /// <summary>
    /// Gets the <see cref="IIdentifier"/>.
    /// </summary>
    IIdentifier Identifier { get; }
}

/// <summary>
/// Represents a binding between an <typeparamref name="TIdentifier"/> and something.
/// </summary>
/// <typeparam name="TIdentifier">The <see cref="Type"/> of the <see cref="IIdentifier{TId}"/>.</typeparam>
/// <typeparam name="TId">The type of the globally unique id of the identifier.</typeparam>
public interface IBinding<out TIdentifier, TId> : IBinding
    where TIdentifier : IIdentifier<TId>
    where TId : ConceptAs<Guid>
{
    /// <summary>
    /// Gets the <typeparamref name="TIdentifier"/>.
    /// </summary>
    new TIdentifier Identifier { get; }
}
