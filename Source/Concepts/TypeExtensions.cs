// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Concepts;

/// <summary>
/// Extension methods for <see cref="Type"/>.
/// </summary>
public static class TypeExtensions
{
    /// <summary>
    /// Checks whether the given <see cref="Type"/> is a <see cref="ConceptAs{TValue}"/>.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to check.</param>
    /// <returns>True if it is a <see cref="ConceptAs{TValue}"/> false if not.</returns>
    public static bool IsConcept(this Type type) => type is not null && ConceptMap.IsConcept(type);
        /// <summary>
    /// Get the type of the value inside a <see cref="ConceptAs{T}"/>.
    /// </summary>
    /// <param name="type"><see cref="Type"/> to get value type from.</param>
    /// <returns>The type of the <see cref="ConceptAs{T}"/> value.</returns>
    public static Type GetConceptValueType(this Type type) => ConceptMap.GetConceptValueType(type);
}
