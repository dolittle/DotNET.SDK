// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;

namespace Dolittle.SDK.Aggregates.Builders;

/// <summary>
/// Defines a system that registers <see cref="AggregateRoot"/> <see cref="Type"/> types.
/// </summary>
public interface IAggregateRootsBuilder
{
    /// <summary>
    /// Associate a <see cref="Type" /> with the <see cref="AggregateRootType" /> given by an attribute.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type" /> to associate with an <see cref="AggregateRootType" />.</typeparam>
    /// <returns>The <see cref="IAggregateRootsBuilder"/> for continuation.</returns>
    /// <remarks>
    /// The type must have a AggregateRoot attribute.
    /// </remarks>
    public IAggregateRootsBuilder Register<T>()
        where T : class;

    /// <summary>
    /// Associate the <see cref="Type" /> with the <see cref="AggregateRootType" /> given by an attribute.
    /// </summary>
    /// <param name="type">The <see cref="Type" /> to associate with an <see cref="AggregateRootType" />.</param>
    /// <returns>The <see cref="IAggregateRootsBuilder"/> for continuation.</returns>
    /// <remarks>
    /// The type must have a AggregateRoot attribute.
    /// </remarks>
    public IAggregateRootsBuilder Register(Type type);

    /// <summary>
    /// Registers all aggregate root classes from an <see cref="Assembly" />.
    /// </summary>
    /// <param name="assembly">The <see cref="Assembly" /> to register the aggregate root classes from.</param>
    /// <returns>The <see cref="IAggregateRootsBuilder" /> for continuation.</returns>
    public IAggregateRootsBuilder RegisterAllFrom(Assembly assembly);
}
