// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace Dolittle.SDK.Events.Redaction;

/// <summary>
/// Tool to get the list of properties that are redacted for a given type
/// </summary>
public static class RedactedType<T> where T : class
{
    /// <summary>
    /// Get the properties that are redacted for a given type. Could be empty
    /// </summary>
    // ReSharper disable once StaticMemberInGenericType
    public static ImmutableDictionary<string, object?> RedactedProperties { get; } = GetRedactableProperties();

    private static ImmutableDictionary<string, object?> GetRedactableProperties()
    {
        var redactableRemovedProperties =
            GetRedactableReplacedProperties()
                .ToImmutableDictionary(pair => pair.Key, pair => pair.Value);
        return redactableRemovedProperties;
    }

    private static IEnumerable<KeyValuePair<string, object?>> GetRedactableReplacedProperties()
    {
        var properties = typeof(T).GetProperties();
        var redactableProperties = properties
            .Where(prop => Attribute.IsDefined(prop, typeof(RedactablePersonalDataAttribute)))
            .Select(ToReplacement)
            .OfType<KeyValuePair<string, object?>>()
            .ToList();

        var nestedCandidates =
            properties.Where(prop =>
                prop.PropertyType.IsClass && prop.PropertyType != typeof(string) &&
                !Attribute.IsDefined(prop, typeof(RedactablePersonalDataAttribute))).ToList();

        if (nestedCandidates.Count == 0)
        {
            return redactableProperties;
        }

        foreach (var nestedProp in nestedCandidates)
        {
            // Get generic version of RedactedType<>
            try
            {
                var redactedType = typeof(RedactedType<>).MakeGenericType(nestedProp.PropertyType);
                // get RedactedProperties from the generic type
                var redactedProperties = redactedType.GetProperty("RedactedProperties")?.GetValue(null);
                if (redactedProperties is ImmutableDictionary<string, object?> nestedProperties)
                {
                    redactableProperties.AddRange(nestedProperties.Select(pair =>
                        new KeyValuePair<string, object?>($"{nestedProp.Name}.{pair.Key}", pair.Value)));
                }
            }
            catch
            {
                // ignored
            }
        }

        return redactableProperties;
    }

    static KeyValuePair<string, object?>? ToReplacement(PropertyInfo prop)
    {
        var attribute = prop.GetCustomAttribute<RedactablePersonalDataAttribute>();
        if (attribute is null)
        {
            return null;
        }

        return new KeyValuePair<string, object?>(prop.Name, attribute.ReplacementValue);
    }
}
