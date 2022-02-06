// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dolittle.SDK.Concepts;

namespace Dolittle.SDK.Projections.Copies;

/// <summary>
/// Represents the name of a projection property.
/// </summary>
/// <param name="Value">The collection name.</param>
public record PropertyName(string Value) : ConceptAs<string>(Value)
{
    /// <summary>
    /// Implicitly converts the <see cref="string"/> to <see cref="PropertyName"/>.
    /// </summary>
    /// <param name="value">The <see cref="string"/>.</param>
    /// <returns>The <see cref="PropertyName"/>.</returns>
    public static implicit operator PropertyName(string value) => value != default ? new PropertyName(value) : null;
    
    /// <summary>
    /// Implicitly converts the <see cref="PropertyName"/> to <see cref="string"/>.
    /// </summary>
    /// <param name="field">The <see cref="PropertyName"/>.</param>
    /// <returns>The <see cref="string"/>.</returns>
    public static implicit operator string(PropertyName field) => field?.Value;

    /// <summary>
    /// Try get all the <see cref="PropertyName"/> to <see cref="PropertyInfo"/> mappings from a <see cref="Type"/>.
    /// </summary>
    /// <typeparam name="TReadModel">The <see cref="Type"/> of the projection read mode,.</typeparam>
    /// <returns>A <see cref="IDictionary{TKey,TValue}"/> with <see cref="PropertyName"/> to <see cref="MemberInfo"/> mappings.</returns>
    public static IDictionary<PropertyName, MemberInfo> GetAllFrom<TReadModel>()
        where TReadModel : class, new()
        => GetAllFrom(typeof(TReadModel));
    
    /// <summary>
    /// Try get all the <see cref="PropertyName"/> to <see cref="PropertyInfo"/> mappings from a <see cref="Type"/>.
    /// </summary>
    /// <param name="type">The projection read model <see cref="Type"/>.</param>
    /// <returns>A <see cref="IDictionary{TKey,TValue}"/> with <see cref="PropertyName"/> to <see cref="MemberInfo"/> mappings.</returns>
    public static IDictionary<PropertyName, MemberInfo> GetAllFrom(Type type)
    {
        const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;
        var members = type
            .GetFields(bindingFlags)
            .Cast<MemberInfo>()
            .Concat(type.GetProperties(bindingFlags).Where(_ => _.CanRead));
        return members.ToDictionary(_ => new PropertyName(_.Name), _ => _);
    }
}
