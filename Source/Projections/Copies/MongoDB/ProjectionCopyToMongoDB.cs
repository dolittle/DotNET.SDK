// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using PbProjectionCopyToMongoDB = Dolittle.Runtime.Events.Processing.Contracts.ProjectionCopyToMongoDB;

namespace Dolittle.SDK.Projections.Copies.MongoDB;

/// <summary>
/// Represents the SDK version of <see cref="PbProjectionCopyToMongoDB"/>.
/// </summary>
/// <param name="ShouldCopy">Whether the projection should be copied to MongoDB.</param>
/// <param name="CollectionName">The <see cref="MongoDBCopyCollectionName"/>.</param>
/// <param name="Conversions">The BsonType per field conversions.</param>
public record ProjectionCopyToMongoDB(bool ShouldCopy, MongoDBCopyCollectionName CollectionName, IEnumerable<PropertyConversion> Conversions)
{
    /// <summary>
    /// The default representation of <see cref="ProjectionCopyToMongoDB"/>.
    /// </summary>
    public static ProjectionCopyToMongoDB Default => new(false, "", Enumerable.Empty<PropertyConversion>());
    
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
        
        result.Conversions.AddRange(Conversions.Select(_ => _.ToProtobuf()));
        return result;
    }
}
