// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Dolittle.SDK.Concepts;
using Dolittle.SDK.Projections.Copies.MongoDB;

namespace Dolittle.SDK.Projections.Copies;

/// <summary>
/// Represents a the path of of a projection property.
/// </summary>
public record PropertyPath : ConceptAs<string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyPath"/>.
    /// </summary>
    /// <param name="value">The property path string.</param>
    /// <exception cref="ProjectionPropertyPathStringCannotBeEmpty">Thrown when given path is null or empty.</exception>
    public PropertyPath(string value)
        : base(value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ProjectionPropertyPathStringCannotBeEmpty();
        }
    }

    /// <summary>
    /// Gets all <see cref="IEnumerable{T}"/> of <see cref="PropertyName"/> representing the parts of the path.
    /// </summary>
    public IEnumerable<PropertyName> GetParts()
        => Value.Split('.').Select(_ => new PropertyName(_));

    /// <summary>
    /// Implicitly converts the <see cref="string"/> to <see cref="PropertyPath"/>.
    /// </summary>
    /// <param name="value">The <see cref="string"/>.</param>
    /// <returns>The <see cref="PropertyPath"/>.</returns>
    public static implicit operator PropertyPath(string value) => new(value);
    
    /// <summary>
    /// Implicitly converts the <see cref="PropertyPath"/> to <see cref="string"/>.
    /// </summary>
    /// <param name="field">The <see cref="PropertyPath"/>.</param>
    /// <returns>The <see cref="string"/>.</returns>
    public static implicit operator string(PropertyPath field) => field.Value;
}
