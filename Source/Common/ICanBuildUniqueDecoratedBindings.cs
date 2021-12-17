// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Common;

/// <summary>
/// Defines a builder for that can build value to unique identifier bindings where the value is a decorated type that identifies itself.
/// </summary>
/// <typeparam name="TIdentifier">The <see cref="Type" /> of the unique identifier.</typeparam>
/// <typeparam name="TValue">The <see cref="Type" /> of the value to associate with the unique identifier.</typeparam>
/// <typeparam name="TUniqueBindings">The <see cref="Type"/> of the <see cref="IUniqueBindings{TIdentifier,TValue}"/> to be built</typeparam>
public interface ICanBuildUniqueDecoratedBindings<in TIdentifier, in TValue, out TUniqueBindings> : ICanBuildUniqueBindings<TIdentifier, TValue, TUniqueBindings>
    where TIdentifier : IEquatable<TIdentifier>
    where TValue : class
    where TUniqueBindings : IUniqueBindings<TIdentifier, TValue>
{
    /// <summary>
    /// Adds a binding between the <typeparamref name="TValue"/> and the <typeparamref name="TIdentifier"/> where the <typeparamref name="TIdentifier"/> is derived from the decorator.
    /// </summary>
    /// <param name="value"><typeparamref name="TValue"/> where a decorator with <see cref="TIdentifier"/> can be derived.</param>
    void Add(TValue value);
}
