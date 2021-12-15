// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;

namespace Dolittle.SDK.Common;

/// <summary>
/// Defines a builder for that can build value to unique identifier bindings where the value is a decorated type that identifies itself.
/// </summary>
/// <typeparam name="TIdentifier">The <see cref="Type" /> of the unique identifier.</typeparam>
/// <typeparam name="TUniqueBindings">The <see cref="Type"/> of the <see cref="IUniqueBindings{TIdentifier,TValue}"/> to be built</typeparam>
public interface ICanBuildUniqueDecoratedBindings<in TIdentifier, out TUniqueBindings> : ICanBuildUniqueBindings<TIdentifier, Type, TUniqueBindings>
    where TIdentifier : IEquatable<TIdentifier>
    where TUniqueBindings : IUniqueBindings<TIdentifier, Type>
{
    /// <summary>
    /// Adds a binding between the <see cref="Type"/> and the <typeparamref name="TIdentifier"/> where the <typeparamref name="TIdentifier"/> is decorated on the <see cref="Type"/>.
    /// </summary>
    /// <param name="type"><see cref="Type"/> decorated with <see cref="TIdentifier"/>.</param>
    void Add(Type type);
    
    /// <summary>
    /// Adds all bindings discovered in the <see cref="Assembly"/>.
    /// </summary>
    /// <param name="assembly"><see cref="Assembly"/> to add all bindings from.</param>
    void AddAllFrom(Assembly assembly);
}
