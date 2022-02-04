// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Dolittle.SDK.Concepts;
using Dolittle.SDK.Projections.Copies.MongoDB;

namespace Dolittle.SDK.Projections.Copies;

/// <summary>
/// Represents a the path of of a projection property represented as a string.
/// </summary>
public record ProjectionPropertyPathString : ConceptAs<string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionPropertyPathString"/>.
    /// </summary>
    /// <param name="value">The property path string.</param>
    /// <exception cref="ProjectionPropertyPathStringCannotBeEmpty">Thrown when given path is null or empty.</exception>
    public ProjectionPropertyPathString(string value)
        : base(value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ProjectionPropertyPathStringCannotBeEmpty();
        }
        Parts = value.Split('.').Select(_ => new ProjectionPropertyName(_));
    }
    
    /// <summary>
    /// Gets all <see cref="IEnumerable{T}"/> of <see cref="ProjectionPropertyName"/> representing the parts of the path.
    /// </summary>
    public IEnumerable<ProjectionPropertyName> Parts { get; }
    
    /// <summary>
    /// Implicitly converts the <see cref="string"/> to <see cref="ProjectionPropertyPathString"/>.
    /// </summary>
    /// <param name="value">The <see cref="string"/>.</param>
    /// <returns>The <see cref="ProjectionPropertyPathString"/>.</returns>
    public static implicit operator ProjectionPropertyPathString(string value) => new(value);
    
    /// <summary>
    /// Implicitly converts the <see cref="ProjectionPropertyPathString"/> to <see cref="string"/>.
    /// </summary>
    /// <param name="field">The <see cref="ProjectionPropertyPathString"/>.</param>
    /// <returns>The <see cref="string"/>.</returns>
    public static implicit operator string(ProjectionPropertyPathString field) => field.Value;
}
