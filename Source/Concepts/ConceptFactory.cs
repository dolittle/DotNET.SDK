// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;

namespace Dolittle.SDK.Concepts;

/// <summary>
/// Factory to create an instance of a <see cref="ConceptAs{T}"/> from the Type and Underlying value.
/// </summary>
public static class ConceptFactory
{
    /// <summary>
    /// Creates an instance of a <see cref="ConceptAs{T}"/> given the type and underlying value.
    /// </summary>
    /// <param name="type">Type of the ConceptAs to create.</param>
    /// <param name="value">Value to give to this instance.</param>
    /// <returns>An instance of a ConceptAs with the specified value.</returns>
    public static object CreateConceptInstance(Type type, object? value)
        => type
            .GetConstructors()
            .Single(c => c.GetParameters().Length > 0)
            .Invoke([value]);
}
