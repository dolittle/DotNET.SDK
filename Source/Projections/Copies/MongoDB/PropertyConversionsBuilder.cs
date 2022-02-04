// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;

namespace Dolittle.SDK.Projections.Copies.MongoDB;

/// <summary>
/// Represents an implementation of <see cref="IPropertyConversionsBuilder"/>.
/// </summary>
public class PropertyConversionsBuilder : IPropertyConversionsBuilder
{
    class PropertyConversionNode
    {
        public ProjectionPropertyName PropertyName { get; }
        public Conversion ConvertTo { get; set; } = Conversion.None;
        public ProjectionPropertyName RenameTo { get; set; }
        public Dictionary<ProjectionPropertyName, PropertyConversionNode> Children { get; } = new();

        public PropertyConversionNode(ProjectionPropertyName propertyName)
        {
            PropertyName = propertyName;
        }

        public PropertyConversion ToConversion()
            => new(PropertyName, Conversion.Guid, Children.Values.Select(_ => _.ToConversion()), RenameTo);
    }

    readonly Dictionary<ProjectionPropertyName, PropertyConversionNode> _conversions = new();

    /// <inheritdoc />
    public void AddConversion(ProjectionPropertyPathString pathString, Conversion conversion)
        => GetNode(pathString).ConvertTo = conversion;

    /// <inheritdoc />
    public void AddRenaming(ProjectionPropertyPathString pathString, ProjectionPropertyName name)
        => GetNode(pathString).RenameTo = name;
    
    /// <inheritdoc />
    public IEnumerable<PropertyConversion> Build()
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
