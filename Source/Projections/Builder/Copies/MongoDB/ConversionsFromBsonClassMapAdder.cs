// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.ApplicationModel.ClientSetup;
using Dolittle.SDK.Projections.Copies;
using Dolittle.SDK.Projections.Copies.MongoDB;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Dolittle.SDK.Projections.Builder.Copies.MongoDB;

/// <summary>
/// Represents an implementation of <see cref="IAddConversionsFromBsonClassMap"/>. 
/// </summary>
public class ConversionsFromBsonClassMapAdder : IAddConversionsFromBsonClassMap
{
    /// <inheritdoc />
    public void AddFrom(BsonClassMap classMap, IClientBuildResults buildResults, IPropertyConversions conversions)
    {
        BuildFromAllMemberMaps(classMap, conversions);
    }

    static void BuildFromAllMemberMaps(BsonClassMap classMap, IPropertyConversions conversions, PropertyPath parentPath = default)
    {
        foreach (var memberMap in classMap.AllMemberMaps)
        {
            BuildFromMemberMap(memberMap, conversions, parentPath == default ? memberMap.MemberName : parentPath.Add(memberMap.MemberName));
        }
    }

    static void BuildFromMemberMap(BsonMemberMap memberMap, IPropertyConversions conversions, PropertyPath path)
    {
        if (memberMap.ElementName != memberMap.MemberName)
        {
            conversions.AddRenaming(path, memberMap.ElementName);
        }
        var serializer = memberMap.GetSerializer();
        if (IsComplexType(serializer))
        {
            BuildFromAllMemberMaps(BsonClassMap.LookupClassMap(serializer.ValueType), conversions, path);
        }
        else if (TryResolveConversionFromSerializer(serializer, out var conversion))
        {
            conversions.AddConversion(path, conversion);
        }
    }
    static bool TryResolveConversionFromSerializer(IBsonSerializer serializer, out Conversion conversion)
    {
        conversion = Conversion.None;
        if (serializer is IBsonArraySerializer arraySerializer && arraySerializer.TryGetItemSerializationInfo(out var info))
        {
            serializer = info.Serializer;
        }
        switch (serializer)
        {
            case GuidSerializer guidSerializer:
                if (guidSerializer.Representation == BsonType.String)
                {
                    conversion = Conversion.GuidAsString;
                    return true;
                }
                else if (guidSerializer.Representation == BsonType.Binary)
                {
                    switch (guidSerializer.GuidRepresentation)
                    {
                        case GuidRepresentation.Standard:
                            conversion = Conversion.GuidAsStandardBinary;
                            return true;
                        case GuidRepresentation.CSharpLegacy:
                        case GuidRepresentation.Unspecified:
                            conversion = Conversion.GuidAsCSharpLegacyBinary;
                            return true;
                    }
                }
                return false;
            case DateTimeSerializer dateTimeSerializer:
                return TryGetConversionFromDateBsonRepresentation(dateTimeSerializer.Representation, out conversion);
                
            case DateTimeOffsetSerializer dateTimeOffsetSerializer:
                return TryGetConversionFromDateBsonRepresentation(dateTimeOffsetSerializer.Representation, out conversion);
            default:
                return false;
        }
    }

    static bool TryGetConversionFromDateBsonRepresentation(BsonType representation, out Conversion conversion)
    {
        conversion = Conversion.None;
        switch (representation)
        {
            case BsonType.Array:
                conversion = Conversion.DateAsArray;
                return true;
            case BsonType.Document:
                conversion = Conversion.DateAsDocument;
                return true;
            case BsonType.DateTime:
                conversion = Conversion.DateAsDate;
                return true;
            case BsonType.String:
                conversion = Conversion.DateAsString;
                return true;
            case BsonType.Int64:
                conversion = Conversion.DateAsInt64;
                return true;
            default:
                return false;
        }
    }
    static bool IsComplexType(IBsonSerializer serializer)
        => serializer.GetType().IsGenericType && serializer.GetType().GetGenericTypeDefinition() == typeof(BsonClassMapSerializer<>);
}
