// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Projections.Copies;
using Dolittle.SDK.Projections.Copies.MongoDB;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Dolittle.SDK.Projections.Builder.Copies.MongoDB;

/// <summary>
/// Represents an implementation of <see cref="ICanBuildPropertyConversionsFromReadModel"/>.
/// </summary>
public class ConversionsFromBsonClassMapBuilder : IBuildPropertyConversionsFromBsonClassMap
{
    /// <inheritdoc />
    public bool TryBuildFrom<TReadModel>(IClientBuildResults buildResults, IPropertyConversions conversions)
        where TReadModel : class, new()
    {
        var classMap = BsonClassMap.LookupClassMap(typeof(TReadModel));
        foreach (var memberMap in classMap.AllMemberMaps)
        {
            BuildFromMemberMap(memberMap, conversions, memberMap.MemberName);
        }
        var idMemberMap = classMap.IdMemberMap;
        if (idMemberMap != default)
        {
            conversions.AddRenaming(idMemberMap.MemberName, "_id");
        }
        return true;
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
        var serializer = memberMap.GetSerializer();
        if (IsComplexType(memberMap.MemberType, serializer))
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
        if (serializer is IBsonArraySerializer arraySerializer)
        {
            if (arraySerializer.TryGetItemSerializationInfo(out var info))
            {
                serializer = info.Serializer;
            }
        }
        switch (serializer)
        {
            case GuidSerializer:
                conversion = Conversion.Guid;
                return true;
            case DateTimeSerializer:
            case DateTimeOffsetSerializer:
                conversion = Conversion.Date;
                return true;
            default:
                return false;
        }
    }

    static bool IsComplexType(Type nominalType, IBsonSerializer serializer) => typeof(BsonClassMapSerializer<>).MakeGenericType(nominalType).IsInstanceOfType(serializer);
}
