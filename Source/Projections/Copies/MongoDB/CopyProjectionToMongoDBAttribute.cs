// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Projections.Copies.MongoDB;

/// <summary>
/// Decorates a projection class to indicate that the projection read models should be copied to a MongoDB store.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class CopyProjectionToMongoDBAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CopyProjectionToMongoDBAttribute"/> class.
    /// </summary>
    /// <param name="collectionName">The optional collection name.</param>
    public CopyProjectionToMongoDBAttribute(string collectionName = "")
    {
        CollectionName = collectionName;
    }

    /// <summary>
    /// Gets the unique identifier for this projection.
    /// </summary>
    public ProjectionMongoDBCopyCollectionName CollectionName { get; }
}
