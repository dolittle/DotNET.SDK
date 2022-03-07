// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Projections.Copies.MongoDB;

namespace Dolittle.SDK.Projections.Builder.Copies.MongoDB.Internal.for_MongoDbCopyDefinitionFromReadModelBuilder.given;

[Projection("AF5ABF69-FD8C-4831-8E0B-B944CC8F60A9")]
public class projection_type_without_mongo_db_copy
{
    public int AValue { get; set; }
}
[Projection("3B1786CF-CDD3-4D00-B517-D2E31617068D")]
[CopyProjectionToMongoDB]
public class projection_type_with_mongo_db_copy
{
    
}
[Projection("55A5A957-AC30-4A1F-9630-04E8D5714E7B")]
[CopyProjectionToMongoDB("some_collection")]
public class projection_type_with_mongo_db_copy_and_collection_name
{
    
}