// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using PbProjectionCopyToMongoDB = Dolittle.Runtime.Events.Processing.Contracts.ProjectionCopyToMongoDB;

namespace Dolittle.SDK.Projections.Copies.MongoDB;

/// <summary>
/// Represents the SDK version of <see cref="PbProjectionCopyToMongoDB"/>.
/// </summary>
/// <param name="ShouldCopy">Whether the projection should be copied to MongoDB.</param>
/// <param name="CollectionName">The <see cref="ProjectionMongoDBCopyCollectionName"/>.</param>
/// <param name="Conversions">The BsonType per field conversions.</param>
public record ProjectionCopyToMongoDB(bool ShouldCopy, ProjectionMongoDBCopyCollectionName CollectionName, IDictionary<ProjectionField, Conversion> Conversions)
{
    /// <summary>
    /// The default representation of <see cref="ProjectionCopyToMongoDB"/>.
    /// </summary>
    public static ProjectionCopyToMongoDB Default => new(false, "", new Dictionary<ProjectionField, Conversion>());
    
    /// <summary>
    /// Creates a Protobuf representation of this <see cref="ProjectionCopyToMongoDB"/>.
    /// </summary>
    /// <returns><see cref="PbProjectionCopyToMongoDB"/>.</returns>
    public PbProjectionCopyToMongoDB ToProtobuf()
    {
        var result = new PbProjectionCopyToMongoDB
        {
            Collection = CollectionName
        };
        
        foreach (var (field, type) in Conversions)
        {
            result.Conversions.Add(field, ToProtobuf(type));
        }
        return result;
    }

    static PbProjectionCopyToMongoDB.Types.BSONType ToProtobuf(Conversion type)
        => type switch
        {
            Conversion.Guid => PbProjectionCopyToMongoDB.Types.BSONType.Binary,
            Conversion.DateTime => PbProjectionCopyToMongoDB.Types.BSONType.Date,
            _ => throw new UnsupportedConversion(type)
        };
}
