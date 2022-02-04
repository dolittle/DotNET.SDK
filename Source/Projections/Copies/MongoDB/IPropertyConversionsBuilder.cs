// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Dolittle.SDK.Projections.Copies.MongoDB;

/// <summary>
/// Defines a builder that can build <see cref="IEnumerable{T}"/> of <see cref="PropertyConversion"/> as multiple tree-structures.
/// </summary>
public interface IPropertyConversionsBuilder
{
    /// <summary>
    /// Adds a conversion.
    /// </summary>
    /// <param name="pathString">The <see cref="ProjectionPropertyPathString"/>.</param>
    /// <param name="conversion">The <see cref="Conversion"/> of the <see cref="PropertyConversion"/>.</param>
    void AddConversion(ProjectionPropertyPathString pathString, Conversion conversion);
    
    /// <summary>
    /// Adds a renaming.
    /// </summary>
    /// <param name="pathString">The <see cref="ProjectionPropertyPathString"/>.</param>
    /// <param name="name">The <see cref="ProjectionPropertyName"/> to rename the <see cref="PropertyConversion"/> to.</param>
    void AddRenaming(ProjectionPropertyPathString pathString, ProjectionPropertyName name);

    /// <summary>
    /// Builds the <see cref="PropertyConversion"/> trees.
    /// </summary>
    /// <returns>The <see cref="IEnumerable{T}"/> of <see cref="PropertyConversion"/>.</returns>
    IEnumerable<PropertyConversion> Build();
}
