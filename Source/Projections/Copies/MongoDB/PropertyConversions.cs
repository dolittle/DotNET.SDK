// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;

namespace Dolittle.SDK.Projections.Copies.MongoDB;

/// <summary>
/// Represents <see cref="PropertyConversion"/> as nodes in a tree-structure where the tree hierarchy is based off <see cref="ProjectionPropertyPathString"/>.
/// </summary>
public class PropertyConversions
{
    class PropertyConversionNode
    {
        readonly ProjectionPropertyName _propertyName;

        public PropertyConversionNode(ProjectionPropertyName propertyName)
        {
            _propertyName = propertyName;
        }
        
        public Conversion ConvertTo { get; set; } = Conversion.None;
        public ProjectionPropertyName RenameTo { get; set; }
        public Dictionary<ProjectionPropertyName, PropertyConversionNode> Children { get; } = new();

        public PropertyConversion ToConversion()
            => new(_propertyName, Conversion.Guid, Children.Values.Select(_ => _.ToConversion()), RenameTo);
    }

    readonly Dictionary<ProjectionPropertyName, PropertyConversionNode> _conversions = new();
    
    /// <summary>
    /// Adds a conversion.
    /// </summary>
    /// <param name="pathString">The <see cref="ProjectionPropertyPathString"/>.</param>
    /// <param name="conversion">The <see cref="Conversion"/> of the <see cref="PropertyConversion"/>.</param>
    public void AddConversion(ProjectionPropertyPathString pathString, Conversion conversion)
        => GetNode(pathString).ConvertTo = conversion;

    /// <summary>
    /// Adds a renaming.
    /// </summary>
    /// <param name="pathString">The <see cref="ProjectionPropertyPathString"/>.</param>
    /// <param name="name">The <see cref="ProjectionPropertyName"/> to rename the <see cref="PropertyConversion"/> to.</param>
    public void AddRenaming(ProjectionPropertyPathString pathString, ProjectionPropertyName name)
        => GetNode(pathString).RenameTo = name;

    /// <summary>
    /// Gets an <see cref="IEnumerable{T}"/> of all the <see cref="PropertyConversion"/> trees.
    /// </summary>
    /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="PropertyConversion"/>.</returns>
    public IEnumerable<PropertyConversion> GetAll()
        => _conversions.Values.Select(_ => _.ToConversion());

    PropertyConversionNode GetNode(ProjectionPropertyPathString pathString)
    {
        PropertyConversionNode node = default;
        foreach (var propertyName in pathString.Parts)
        {
            node = GetOrAddNode(node?.Children ?? _conversions, propertyName);
        }
        return node;
    }

    static PropertyConversionNode GetOrAddNode(Dictionary<ProjectionPropertyName, PropertyConversionNode> parent, ProjectionPropertyName rootName)
    {
        if (parent.TryGetValue(rootName, out var node))
        {
            return node;
        }
        node = new PropertyConversionNode(rootName);
        parent.Add(rootName, node);
        return node;
    }
}
