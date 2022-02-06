// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Projections.Copies;
using Dolittle.SDK.Projections.Copies.MongoDB;

namespace Dolittle.SDK.Projections.Builder.Copies.MongoDB;

/// <summary>
/// Represents an implementation of <see cref="ICanBuildPropertyConversionsFromReadModel"/>.
/// </summary>
public class ConversionsFromMongoDBConvertToAttributesBuilder : IBuildPropertyConversionsFromMongoDBConvertToAttributes
{
    /// <inheritdoc />
    public bool TryBuildFrom<TReadModel>(IClientBuildResults buildResults, IPropertyConversions conversions)
        where TReadModel : class, new()
    {
        foreach (var (path, conversion) in GetConversionsFromType(typeof(TReadModel), new HashSet<Type>()))
        {
            conversions.AddConversion(path, conversion);
        }
        return true;
    }

    Dictionary<PropertyPath, Conversion> GetConversionsFromType(Type type, HashSet<Type> checkedTypes)
    {
        var conversions = new Dictionary<PropertyPath, Conversion>();
        foreach (var property in type.GetProperties().Where(_ => !checkedTypes.Contains(_.PropertyType)))
        {
            var attribute = property.GetCustomAttribute<MongoDBConvertToAttribute>();
            if (attribute is null)
            {
                continue;
            }
            conversions[property.Name] = attribute.Conversion;
            foreach (var (childProperty, childConversion) in GetConversionsFromType(property.PropertyType, checkedTypes.Append(type).ToHashSet()))
            {
                conversions[string.Join('.', property.Name, childProperty)] = childConversion;
            }
        }
        foreach (var field in type.GetFields().Where(_ => !checkedTypes.Contains(_.FieldType)))
        {
            var attribute = field.GetCustomAttribute<MongoDBConvertToAttribute>();
            if (attribute is null)
            {
                continue;
            }
            conversions[field.Name] = attribute.Conversion;
            foreach (var (childProperty, childConversion) in GetConversionsFromType(field.FieldType, checkedTypes.Append(type).ToHashSet()))
            {
                conversions[string.Join('.', field.Name, childProperty)] = childConversion;
            }
        }
        return conversions;
    }
}
