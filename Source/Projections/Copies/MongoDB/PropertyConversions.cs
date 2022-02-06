// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;

namespace Dolittle.SDK.Projections.Copies.MongoDB;

/// <summary>
/// Represents an implementation of <see cref="IPropertyConversions"/> using <see cref="PropertyConversion"/> as nodes in a tree-structure where the tree hierarchy is based off <see cref="PropertyPath"/>.
/// </summary>
public class PropertyConversions : IPropertyConversions
{
    class PropertyConversionNode
    {
        readonly PropertyName _propertyName;

        public PropertyConversionNode(PropertyName propertyName)
        {
            _propertyName = propertyName;
        }
        
        public Conversion ConvertTo { get; set; } = Conversion.None;
        public PropertyName RenameTo { get; set; }
        public Dictionary<PropertyName, PropertyConversionNode> Children { get; } = new();

        public PropertyConversion ToConversion()
            => new(_propertyName, ConvertTo, Children.Values.Select(_ => _.ToConversion()), RenameTo);
    }

    readonly Dictionary<PropertyName, PropertyConversionNode> _conversions = new();
    
    /// <summary>
    /// Adds a conversion.
    /// </summary>
    /// <param name="path">The <see cref="PropertyPath"/>.</param>
    /// <param name="conversion">The <see cref="Conversion"/> of the <see cref="PropertyConversion"/>.</param>
    public void AddConversion(PropertyPath path, Conversion conversion)
        => GetNode(path).ConvertTo = conversion;

    /// <summary>
    /// Adds a renaming.
    /// </summary>
    /// <param name="path">The <see cref="PropertyPath"/>.</param>
    /// <param name="name">The <see cref="PropertyName"/> to rename the <see cref="PropertyConversion"/> to.</param>
    public void AddRenaming(PropertyPath path, PropertyName name)
        => GetNode(path).RenameTo = name;

    /// <summary>
    /// Gets an <see cref="IEnumerable{T}"/> of all the <see cref="PropertyConversion"/> trees.
    /// </summary>
    /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="PropertyConversion"/>.</returns>
    public IEnumerable<PropertyConversion> GetAll()
        => _conversions.Values.Select(_ => _.ToConversion());

    PropertyConversionNode GetNode(PropertyPath path)
    {
        PropertyConversionNode node = default;
        foreach (var propertyName in path.GetParts())
        {
            node = GetOrAddNode(node?.Children ?? _conversions, propertyName);
        }
        return node;
    }

    static PropertyConversionNode GetOrAddNode(Dictionary<PropertyName, PropertyConversionNode> parent, PropertyName rootName)
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
