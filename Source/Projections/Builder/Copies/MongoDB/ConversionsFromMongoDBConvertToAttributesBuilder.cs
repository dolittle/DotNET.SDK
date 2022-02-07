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
        AddConversionsFromType(typeof(TReadModel), new HashSet<Type>(), conversions);
        return true;
    }

    static void AddConversionsFromType(Type type, HashSet<Type> checkedTypes, IPropertyConversions conversions, PropertyPath parentPath = default)
    {
        foreach (var member in type.GetMembers())
        {
            var attribute = member.GetCustomAttribute<MongoDBConvertToAttribute>();
            var hasAttribute = attribute != default;
            if (!TryGetMemberNameAndTypeToCheck(member, out var typeToCheck, out var propertyName) || checkedTypes.Contains(typeToCheck))
            {
                continue;
            }
            var path = parentPath != default ? parentPath.Add(propertyName) : new PropertyPath(propertyName);
            if (hasAttribute)
            {
                conversions.AddConversion(path, attribute.Conversion);
            }
            AddConversionsFromType(typeToCheck, checkedTypes.Append(type).ToHashSet(), conversions, path);
        }
    }

    static bool TryGetMemberNameAndTypeToCheck(MemberInfo member, out Type typeToCheck, out PropertyName memberName)
    {
        typeToCheck = default;
        memberName = default;
        switch (member)
        {
            case PropertyInfo property:
                typeToCheck = property.PropertyType;
                memberName = property.Name;
                return true;
            case FieldInfo field:
                typeToCheck = field.FieldType;
                memberName = field.Name;
                return true;
            default:
                return false;
        }
    }
}
