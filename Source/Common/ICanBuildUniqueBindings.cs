// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Common.ClientSetup;

namespace Dolittle.SDK.Common;

/// <summary>
/// Defines a builder for that can build value to unique identifier bindings.
/// </summary>
/// <typeparam name="TIdentifier">The <see cref="Type" /> of the unique identifier.</typeparam>
/// <typeparam name="TValue">The <see cref="Type" /> of the value to associate with the unique identifier.</typeparam>
/// <typeparam name="TUniqueBindings">The <see cref="Type"/> of the <see cref="IUniqueBindings{TIdentifier,TValue}"/> to be built</typeparam>
public interface ICanBuildUniqueBindings<in TIdentifier, in TValue, out TUniqueBindings>
    where TIdentifier : IEquatable<TIdentifier>
    where TValue : class
    where TUniqueBindings : IUniqueBindings<TIdentifier, TValue>
{
    /// <summary>
    /// Adds a binding between a value and an unique identifier.
    /// </summary>
    /// <param name="identifier">The <typeparamref name="TIdentifier"/> associated with the <typeparamref name="TValue"/>.</param>
    /// <param name="value">The <typeparamref name="TValue"/> to associate with the <typeparamref name="TIdentifier"/>.</param>
    void Add(TIdentifier identifier, TValue value);

    /// <summary>
    /// Builds the <typeparamref name="TUniqueBindings"/>.
    /// </summary>
    /// <param name="buildResults">The <see cref="IClientBuildResults"/>.</param>
    /// <returns>The <typeparamref name="TUniqueBindings"/>.</returns>
    TUniqueBindings Build(IClientBuildResults buildResults);
}
