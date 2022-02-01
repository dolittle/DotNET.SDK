// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using MongoDB.Bson;
using PbProjectionCopyToMongoDB = Dolittle.Runtime.Events.Processing.Contracts.ProjectionCopyToMongoDB;

namespace Dolittle.SDK.Projections.Copies.MongoDB;

/// <summary>
/// Represents the SDK version of <see cref="PbProjectionCopyToMongoDB"/>.
/// </summary>
/// <param name="ShouldCopy">Whether the projection should be copied to MongoDB.</param>
/// <param name="CollectionName">The <see cref="ProjectionMongoDBCopyCollectionName"/>.</param>
/// <param name="Conversions">The BsonType per field conversions.</param>
public record ProjectionCopyToMongoDB(bool ShouldCopy, ProjectionMongoDBCopyCollectionName CollectionName, IDictionary<string, BsonType> Conversions)
{
    /// <summary>
    /// Gets the default <see cref="ProjectionCopyToMongoDB"/>.
    /// </summary>
    public static ProjectionCopyToMongoDB Default => new(false, "", new Dictionary<string, BsonType>());

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
        
        foreach (var (fieldName, type) in Conversions)
        {
            result.Conversions.Add(fieldName, ToProtobuf(type));
        }
        return result;
    }

    static PbProjectionCopyToMongoDB.Types.BSONType ToProtobuf(BsonType type)
        => type switch
        {
            BsonType.Binary => PbProjectionCopyToMongoDB.Types.BSONType.Binary,
            BsonType.DateTime => PbProjectionCopyToMongoDB.Types.BSONType.Date,
            BsonType.Timestamp => PbProjectionCopyToMongoDB.Types.BSONType.Timestamp,
            _ => throw new UnsupportedBSONType(type)
        };
}
