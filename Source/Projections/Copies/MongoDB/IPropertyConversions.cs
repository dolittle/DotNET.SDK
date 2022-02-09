// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Dolittle.SDK.Projections.Copies.MongoDB;

/// <summary>
/// Defines a system that knows about <see cref="PropertyConversion"/>.
/// </summary>
public interface IPropertyConversions
{
    /// <summary>
    /// Adds a conversion.
    /// </summary>
    /// <param name="path">The <see cref="PropertyPath"/>.</param>
    /// <param name="conversion">The <see cref="Conversion"/> of the <see cref="PropertyConversion"/>.</param>
    void AddConversion(PropertyPath path, Conversion conversion);

    /// <summary>
    /// Adds a renaming.
    /// </summary>
    /// <param name="path">The <see cref="PropertyPath"/>.</param>
    /// <param name="name">The <see cref="PropertyName"/> to rename the <see cref="PropertyConversion"/> to.</param>
    void AddRenaming(PropertyPath path, PropertyName name);

    /// <summary>
    /// Gets an <see cref="IEnumerable{T}"/> of all the <see cref="PropertyConversion"/> trees.
    /// </summary>
    /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="PropertyConversion"/>.</returns>
    IEnumerable<PropertyConversion> GetAll();
}
