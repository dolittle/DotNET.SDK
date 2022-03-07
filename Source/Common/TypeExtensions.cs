// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Common;

/// <summary>
/// Represents handy extensions for <see cref="Type"/>.
/// </summary>
public static class TypeExtensions
{
    /// <summary>
    /// Try get an attribute of a specific <see cref="Type"/>.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to get the <typeparamref name="TDecorator"/> <see cref="Attribute"/> from.</param>
    /// <param name="decorator">The outputted <typeparamref name="TDecorator"/>.</param>
    /// <typeparam name="TDecorator">The <see cref="Type"/> of the <see cref="Attribute"/> to get.</typeparam>
    /// <returns>A value indicating whether an <see cref="Attribute"/> of type <typeparamref name="TDecorator"/> exists on <see cref="Type"/>.</returns>
    public static bool TryGetDecorator<TDecorator>(this Type type, out TDecorator decorator)
        where TDecorator : Attribute
    {
        decorator = default;
        if (Attribute.GetCustomAttribute(type, typeof(TDecorator)) is not TDecorator decoratorOnType)
        {
            return false;
        }
        decorator = decoratorOnType;
        return true;
    }
}
