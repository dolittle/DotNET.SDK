// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Projections.Copies;
using Dolittle.SDK.Projections.Copies.MongoDB;
using MongoDB.Bson.Serialization;

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
        if (typeof(BsonClassMapSerializer<>).MakeGenericType(memberMap.MemberType).IsInstanceOfType(serializer))
        {
            BuildFromAllMemberMaps(BsonClassMap.LookupClassMap(serializer.ValueType), conversions, path);
        }
        var memberType = GetTypeOrElementType(memberMap.MemberType);
        if (memberType == typeof(Guid))
        {
            conversions.AddConversion(path, Conversion.Guid);
        }
        else if (memberType == typeof(DateTime) || memberMap.MemberType == typeof(DateTimeOffset))
        {
            conversions.AddConversion(path, Conversion.Date);
        }

    }
    static Type GetTypeOrElementType(Type type)
    {
        if (!typeof(IEnumerable).IsAssignableFrom(type))
        {
            return type;
        }
        var interfaces = type.GetInterfaces().Append(type);
        var elementType = (from i in interfaces
                            where i.IsGenericType && i.GetGenericTypeDefinition() == typeof (IEnumerable<>)
                            select i.GetGenericArguments()[0]).FirstOrDefault();
        
        return elementType ?? type;
    }
}
