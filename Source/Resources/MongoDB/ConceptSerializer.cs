// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Reflection;
using Dolittle.SDK.Concepts;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;

namespace Dolittle.SDK.Resources.MongoDB;

/// <summary>
/// Represents a <see cref="IBsonSerializer{T}"/> for <see cref="ConceptAs{T}"/> types.
/// </summary>
/// <typeparam name="T">Type of concept.</typeparam>
public class ConceptSerializer<T> : IBsonSerializer<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConceptSerializer{T}"/> class.
    /// </summary>
    public ConceptSerializer()
    {
        ValueType = typeof(T);
        if (!ValueType.IsConcept())
        {
            throw new ArgumentException($"Type {ValueType} is not a concept-type");
        }
    }

    /// <inheritdoc/>
    public Type ValueType { get; }

    /// <inheritdoc/>
    public T Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var bsonReader = context.Reader;
        var actualType = args.NominalType;
        var bsonType = bsonReader.GetCurrentBsonType();
        var valueType = actualType.GetConceptValueType();

        object value;

        // It should be a Concept object
        if (bsonType == BsonType.Document)
        {
            bsonReader.ReadStartDocument();
            var keyName = bsonReader.ReadName(Utf8NameDecoder.Instance);
            if (keyName is "Value" or "value")
            {
                value = GetDeserializedValue(valueType, ref bsonReader);
                bsonReader.ReadEndDocument();
            }
            else
            {
                throw new FailedConceptSerialization("Expected a concept object, but no key named 'Value' or 'value' was found on the object");
            }
        }
        else
        {
            value = GetDeserializedValue(valueType, ref bsonReader);
        }

        return (T)ValueType.GetConstructors().Single(_ => _.GetParameters().Length == 1).Invoke(new[]{value});
    }

    /// <inheritdoc/>
    public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
    {
        var nominalType = args.NominalType;
        var underlyingValueType = nominalType.GetConceptValueType();
        var underlyingValue = value.GetType().GetTypeInfo().GetProperty(nameof(ConceptAs<string>.Value))!.GetValue(value, null);
        var bsonWriter = context.Writer;
        BsonSerializer.Serialize(bsonWriter, underlyingValueType, underlyingValue);
    }

    /// <inheritdoc/>
    public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, T value)
        => Serialize(context, args, (object)value!);

    /// <inheritdoc/>
    object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        => Deserialize(context, args)!;

    static object GetDeserializedValue(Type valueType, ref IBsonReader bsonReader)
    {
        return BsonSerializer.Deserialize(bsonReader, valueType);
    }
}
