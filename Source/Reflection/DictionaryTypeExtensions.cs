// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dolittle.SDK.Reflection
{
    /// <summary>
    /// Provides a set of methods for working with <see cref="Type">dictionary types</see>.
    /// </summary>
    public static class DictionaryTypeExtensions
    {
        static readonly HashSet<Type> GenericDictionaryInterfaces = new HashSet<Type>
        {
            typeof(IDictionary<,>),
            typeof(IReadOnlyDictionary<,>)
        };

        static readonly HashSet<Type> NonGenericDictionaryInterfaces = new HashSet<Type>
        {
            typeof(IDictionary),
        };

        static readonly HashSet<Type> DictionaryInterfaces;

        static DictionaryTypeExtensions()
        {
            var interfaces = new List<Type>();
            interfaces.AddRange(NonGenericDictionaryInterfaces);
            interfaces.AddRange(GenericDictionaryInterfaces);
            DictionaryInterfaces = new HashSet<Type>(interfaces);
        }

        /// <summary>
        /// Check if a type is a dictionary.
        /// </summary>
        /// <param name="type"><see cref="Type"/> to get from.</param>
        /// <returns>True if it is is, false if not.</returns>
        public static bool IsDictionary(this Type type)
        {
            // https://stackoverflow.com/a/29649496
            return type.GetInterfaces().Append(type).Any(t => DictionaryInterfaces.Any(i => i == t || (t.GetTypeInfo().IsGenericType && i == t.GetGenericTypeDefinition())));
        }

        /// <summary>
        /// Get the dictionary type from a type or its inheritance chain.
        /// </summary>
        /// <param name="type"><see cref="Type"/> to get from.</param>
        /// <returns>Type - null if none.</returns>
        public static Type GetDictionaryType(this Type type)
        {
            return type.GetInterfaces().Append(type).FirstOrDefault(t => DictionaryInterfaces.Any(i => i == t || (t.GetTypeInfo().IsGenericType && i == t.GetGenericTypeDefinition())));
        }

        /// <summary>
        /// Get wether or not a dictionary type is readonly or not.
        /// </summary>
        /// <param name="type"><see cref="Type"/> to check.</param>
        /// <returns>True if it is, false if not.</returns>
        public static bool IsReadOnlyDictionary(this Type type)
        {
            var keyType = type.GetKeyTypeFromDictionary();
            var valueType = type.GetValueTypeFromDictionary();
            if (typeof(Dictionary<,>).MakeGenericType(keyType, valueType).IsAssignableFrom(type))
                return false;

            var readOnlyDictionaryType = typeof(IReadOnlyDictionary<,>);
            var interfaces = type.GetInterfaces().Append(type);
            return interfaces.Any(t =>
                t == readOnlyDictionaryType ||
                (t.GetTypeInfo().IsGenericType &&
                t.GetGenericTypeDefinition() == readOnlyDictionaryType));
        }

        /// <summary>
        /// Get the key type from a dictionary type - this is based on it being a generic dictionary.
        /// </summary>
        /// <param name="type"><see cref="Type"/> to get from.</param>
        /// <returns>Type of key.</returns>
        public static Type GetKeyTypeFromDictionary(this Type type)
        {
            var keyType = type.GetDictionaryType().GetGenericArguments()[0];
            return keyType;
        }

        /// <summary>
        /// Get the value type from a dictionary type - this is based on it being a generic dictionary.
        /// </summary>
        /// <param name="type"><see cref="Type"/> to get from.</param>
        /// <returns>Type of key.</returns>
        public static Type GetValueTypeFromDictionary(this Type type)
        {
            var keyType = type.GetDictionaryType().GetGenericArguments()[1];
            return keyType;
        }
    }
}
