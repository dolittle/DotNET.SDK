// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Dolittle.SDK.ApplicationModel;

/// <summary>
/// Extension methods for <see cref="Type"/>.
/// </summary>
public static class TypeExtensions
{
    /// <summary>
    /// Try get <typeparamref name="TIdentifier"/> from <see cref="Attribute"/> extending <see cref="IDecoratedTypeDecorator{TIdentifier}"/> on the given <see cref="Type"/>. 
    /// </summary>
    /// <param name="decoratedType">The <see cref="Type"/> to get the <see cref="Attribute" />.</param>
    /// <param name="identifier">The <typeparamref name="TIdentifier"/>.</param>
    /// <typeparam name="TIdentifier">The <see cref="Type"/> of the <see cref="IIdentifier"/>.</typeparam>
    /// <returns>True id <see cref="Type"/> has the correct <see cref="Attribute"/>, false if not.</returns>
    public static bool TryGetIdentifier<TIdentifier>(this Type decoratedType, [NotNullWhen(true)]out TIdentifier? identifier)
        where TIdentifier : IIdentifier
    {
        identifier = default;
        if (decoratedType.GetCustomAttributes(false)
                .SingleOrDefault(_ => _ is IDecoratedTypeDecorator<TIdentifier>) is not IDecoratedTypeDecorator<TIdentifier> decorator)
        {
            return false;
        }
        identifier = decorator.GetIdentifier(decoratedType);
        return true;
    }
}
