// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using PbProjectionCopyToMongoDB = Dolittle.Runtime.Events.Processing.Contracts.ProjectionCopyToMongoDB;

namespace Dolittle.SDK.Projections.Copies.MongoDB;

public record ProjectionCopyToMongoDB(ProjectionMongoDBCopyCollectionName CollectionName, IDictionary<string, PbProjectionCopyToMongoDB.Types.BSONType> Conversions)
{
    public PbProjectionCopyToMongoDB ToProtobuf()
    {
        var result = new PbProjectionCopyToMongoDB
        {
            Collection = CollectionName    
        };
        
        foreach (var (fieldName, type) in Conversions)
        {
            result.Conversions.Add(fieldName, type);
        }
        return result;
    }
}

