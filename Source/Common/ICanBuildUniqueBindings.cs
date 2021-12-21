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
public interface ICanBuildUniqueBindings<TIdentifier, TValue>
    where TIdentifier : IEquatable<TIdentifier>
    where TValue : class
{
    /// <summary>
    /// Adds a binding between a value and an unique identifier.
    /// </summary>
    /// <param name="identifier">The <typeparamref name="TIdentifier"/> associated with the <typeparamref name="TValue"/>.</param>
    /// <param name="value">The <typeparamref name="TValue"/> to associate with the <typeparamref name="TIdentifier"/>.</param>
    void Add(TIdentifier identifier, TValue value);

    /// <summary>
    /// Builds the <see cref="IUniqueBindings{TIdentifier,TValue}"/>.
    /// </summary>
    /// <param name="buildResults">The <see cref="IClientBuildResults"/>.</param>
    /// <returns>The <see cref="IUniqueBindings{TIdentifier,TValue}"/>.</returns>
    IUniqueBindings<TIdentifier, TValue> Build(IClientBuildResults buildResults);
}
