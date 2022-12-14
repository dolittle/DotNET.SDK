// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Reflection;
using Dolittle.SDK.Concepts;

namespace Dolittle.SDK.Concepts;

/// <summary>
/// Maps a concept type to the underlying primitive type.
/// </summary>
public static class ConceptMap
{
    static readonly ConcurrentDictionary<Type, Type> _cache = new();

    /// <summary>
    /// Get the type of the value in a <see cref="ConceptAs{T}"/>.
    /// </summary>
    /// <param name="type"><see cref="Type"/> to get value type from.</param>
    /// <returns>The type of the <see cref="ConceptAs{T}"/> value.</returns>
    public static Type GetConceptValueType(Type type)
    {
        if (!IsConcept(type))
        {
            throw new TypeIsNotAConcept(type);
        }
        return _cache.GetOrAdd(type, GetPrimitiveTypeFromConcept);
    }
    
    /// <summary>
    /// Checks whether the given <see cref="Type"/> is a <see cref="ConceptAs{TValue}"/>.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to check.</param>
    /// <returns>True if it is a <see cref="ConceptAs{TValue}"/> false if not.</returns>
    public static bool IsConcept(Type type)
    {
        if (_cache.TryGetValue(type, out _))
        {
            return true;
        }

        if (!IsTypeConcept(type))
        {
            return false;
        }
        _cache.TryAdd(type, GetPrimitiveTypeFromConcept(type));
        return true;
    }

    static bool IsTypeConcept(Type? type)
    {
        while (true)
        {
            if (type == null)
            {
                return false;
            }
            if (type.GetTypeInfo().IsGenericType && type.GetTypeInfo().GetGenericTypeDefinition() == typeof(ConceptAs<>))
            {
                return true;
            }
            type = type.BaseType;
        }
    }


    static Type GetPrimitiveTypeFromConcept(Type type)
    {
        var conceptType = type;
        while (true)
        {
            if (conceptType is null || conceptType == typeof(ConceptAs<>))
            {
                break;
            }

            var typeProperty = conceptType.GetTypeInfo().GetProperty(nameof(ConceptAs<string>.UnderlyingType));
            if (typeProperty != null)
            {
                return (Type)typeProperty.GetValue(null)!;
            }

            if (conceptType == typeof(object))
            {
                break;
            }

            conceptType = conceptType.GetTypeInfo().BaseType;
        }

        throw new TypeIsNotAConcept(type);

    }
}
