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

    // private static IEnumerable<KeyValuePair<string, object?>> GetRemovedProperties()
    // {
    //     return typeof(T).GetProperties()
    //         .Where(prop => Attribute.IsDefined(prop, typeof(RedactablePersonalDataAttribute)))
    //         .Select(it => new KeyValuePair<string, object?>(it.Name, it));
    // }

    private static IEnumerable<KeyValuePair<string, object?>> GetRedactableReplacedProperties()
    {
        return typeof(T).GetProperties()
            .Where(prop => Attribute.IsDefined(prop, typeof(RedactablePersonalDataAttribute)))
            .Select(ToReplacement)
            .OfType<KeyValuePair<string, object?>>();
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
