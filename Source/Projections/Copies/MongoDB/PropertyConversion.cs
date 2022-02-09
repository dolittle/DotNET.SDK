// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;

namespace Dolittle.SDK.Projections.Copies.MongoDB;

/// <summary>
/// Represents a projection property conversion.
/// </summary>
/// <param name="PropertyName">The projection property name.</param>
/// <param name="ConvertTo">The <see cref="Conversion"/> type of this projection property.</param>
/// <param name="Children">The <see cref="IEnumerable{T}"/> of <see cref="PropertyConversion"/> child properties.</param>
/// <param name="RenameTo">The new name this projection property should be renamed to.</param>
public record PropertyConversion(PropertyName PropertyName, Conversion ConvertTo, IEnumerable<PropertyConversion> Children, PropertyName RenameTo = default)
{
    /// <summary>
    /// Converts to <see cref="Runtime.Events.Processing.Contracts.ProjectionCopyToMongoDB.Types.PropertyConversion"/>.
    /// </summary>
    /// <returns>The converted <see cref="Runtime.Events.Processing.Contracts.ProjectionCopyToMongoDB.Types.PropertyConversion"/>.</returns>
    public Runtime.Events.Processing.Contracts.ProjectionCopyToMongoDB.Types.PropertyConversion ToProtobuf()
    {
        var result = new Runtime.Events.Processing.Contracts.ProjectionCopyToMongoDB.Types.PropertyConversion()
        {
            PropertyName = PropertyName,
            ConvertTo = ToProtobuf(ConvertTo),
            RenameTo = RenameTo
        };
        
        result.Children.AddRange(Children.Select(_ => _.ToProtobuf()));
        
        return result;
    }

    static Runtime.Events.Processing.Contracts.ProjectionCopyToMongoDB.Types.BSONType ToProtobuf(Conversion type)
        => type switch
        {
            Conversion.None => Runtime.Events.Processing.Contracts.ProjectionCopyToMongoDB.Types.BSONType.None,
            Conversion.GuidAsString => Runtime.Events.Processing.Contracts.ProjectionCopyToMongoDB.Types.BSONType.GuidasString,
            Conversion.GuidAsStandardBinary => Runtime.Events.Processing.Contracts.ProjectionCopyToMongoDB.Types.BSONType.GuidasStandardBinary,
            Conversion.GuidAsCSharpLegacyBinary => Runtime.Events.Processing.Contracts.ProjectionCopyToMongoDB.Types.BSONType.GuidasCsharpLegacyBinary,
            Conversion.DateAsArray => Runtime.Events.Processing.Contracts.ProjectionCopyToMongoDB.Types.BSONType.DateAsArray,
            Conversion.DateAsDate => Runtime.Events.Processing.Contracts.ProjectionCopyToMongoDB.Types.BSONType.DateAsDate,
            Conversion.DateAsDocument => Runtime.Events.Processing.Contracts.ProjectionCopyToMongoDB.Types.BSONType.DateAsDocument,
            Conversion.DateAsInt64 => Runtime.Events.Processing.Contracts.ProjectionCopyToMongoDB.Types.BSONType.DateAsInt64,
            Conversion.DateAsString => Runtime.Events.Processing.Contracts.ProjectionCopyToMongoDB.Types.BSONType.DateAsString,
            _ => throw new UnsupportedConversion(type)
        };
}

