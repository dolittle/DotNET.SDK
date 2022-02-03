// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dolittle.SDK.Concepts;

namespace Dolittle.SDK.Projections.Copies;

/// <summary>
/// Represents a filed of a projection read model.
/// </summary>
/// <param name="Value">The collection name.</param>
public record ProjectionField(string Value) : ConceptAs<string>(Value)
{
    /// <summary>
    /// Implicitly converts the <see cref="string"/> to <see cref="ProjectionField"/>.
    /// </summary>
    /// <param name="value">The <see cref="string"/>.</param>
    /// <returns>The <see cref="ProjectionField"/>.</returns>
    public static implicit operator ProjectionField(string value) => value != default ? new ProjectionField(value) : null;
    
    /// <summary>
    /// Implicitly converts the <see cref="ProjectionField"/> to <see cref="string"/>.
    /// </summary>
    /// <param name="field">The <see cref="ProjectionField"/>.</param>
    /// <returns>The <see cref="string"/>.</returns>
    public static implicit operator string(ProjectionField field) => field?.Value;

    /// <summary>
    /// Try get all the <see cref="ProjectionField"/> to <see cref="PropertyInfo"/> mappings from a <see cref="Type"/>.
    /// </summary>
    /// <typeparam name="TReadModel">The <see cref="Type"/> of the projection read mode,.</typeparam>
    /// <returns>A <see cref="IDictionary{TKey,TValue}"/> with <see cref="ProjectionField"/> to <see cref="PropertyInfo"/> mappings.</returns>
    public static IDictionary<ProjectionField, MemberInfo> GetAllFrom<TReadModel>()
        where TReadModel : class, new()
        => GetAllFrom(typeof(TReadModel));
    
    /// <summary>
    /// Try get all the <see cref="ProjectionField"/> to <see cref="PropertyInfo"/> mappings from a <see cref="Type"/>.
    /// </summary>
    /// <param name="type">The projection read model <see cref="Type"/>.</param>
    /// <returns>A <see cref="IDictionary{TKey,TValue}"/> with <see cref="ProjectionField"/> to <see cref="PropertyInfo"/> mappings.</returns>
    public static IDictionary<ProjectionField, MemberInfo> GetAllFrom(Type type)
    {
        const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;
        var members = type
            .GetFields(bindingFlags)
            .Cast<MemberInfo>()
            .Concat(type.GetProperties(bindingFlags).Where(_ => _.CanRead));
        return members.ToDictionary(_ => new ProjectionField(_.Name), _ => _);
    }
}
