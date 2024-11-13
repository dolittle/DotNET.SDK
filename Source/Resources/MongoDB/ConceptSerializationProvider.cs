// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Concepts;
using MongoDB.Bson.Serialization;

namespace Dolittle.SDK.Resources.MongoDB;

/// <summary>
/// Represents a <see cref="IBsonSerializationProvider"/> for concepts.
/// </summary>
public class ConceptSerializationProvider : IBsonSerializationProvider
{
    /// <summary>
    /// Creates an instance of a serializer of the concept of the given type param T.
    /// </summary>
    /// <typeparam name="T">The Concept type.</typeparam>
    /// <returns><see cref="ConceptSerializer{T}"/> for the specific type.</returns>
    public static ConceptSerializer<T> CreateConceptSerializer<T>()
        => new();

    /// <inheritdoc/>
    public IBsonSerializer? GetSerializer(Type type)
    {
        if (!type.IsConcept())
        {
            return null;
        }
#pragma warning disable CS8602
        var createConceptSerializerGenericMethod = GetType().GetMethod("CreateConceptSerializer").MakeGenericMethod(type);
#pragma warning restore CS8602
#pragma warning disable CS8600
        return (dynamic)createConceptSerializerGenericMethod.Invoke(null, []);
#pragma warning restore CS8600

    }
}
