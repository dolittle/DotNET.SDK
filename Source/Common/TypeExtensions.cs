// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Common;

public static class TypeExtensions
{
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
