// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Common;
using Dolittle.SDK.Concepts;

namespace Dolittle.SDK.Projections.Copies.MongoDB;

/// <summary>
/// Represents the MongoDB copy collection name for a projection.
/// </summary>
/// <param name="Value">The collection name.</param>
public record ProjectionMongoDBCopyCollectionName(string Value) : ConceptAs<string>(Value)
{
    /// <summary>
    /// Implicitly converts the <see cref="string"/> to <see cref="ProjectionMongoDBCopyCollectionName"/>.
    /// </summary>
    /// <param name="value">The <see cref="string"/>.</param>
    /// <returns>The <see cref="ProjectionMongoDBCopyCollectionName"/>.</returns>
    public static implicit operator ProjectionMongoDBCopyCollectionName(string value) => new(value);
    
    /// <summary>
    /// Implicitly converts the <see cref="ProjectionMongoDBCopyCollectionName"/> to <see cref="string"/>.
    /// </summary>
    /// <param name="name">The <see cref="ProjectionMongoDBCopyCollectionName"/>.</param>
    /// <returns>The <see cref="string"/>.</returns>
    public static implicit operator string(ProjectionMongoDBCopyCollectionName name) => name?.Value;

    /// <summary>
    /// Try get the <see cref="ProjectionMongoDBCopyCollectionName"/> from a <see cref="Type"/>.
    /// </summary>
    /// <typeparam name="TProjection">The <see cref="Type"/> of the projection.</typeparam>
    /// <returns>The <see cref="ProjectionMongoDBCopyCollectionName"/> derived from the <see cref="CopyProjectionToMongoDBAttribute"/> or the <see cref="Type.Name"/>.</returns>
    public static ProjectionMongoDBCopyCollectionName GetFrom<TProjection>()
        where TProjection : class, new()
        => GetFrom(typeof(TProjection));
    
    /// <summary>
    /// Try get the <see cref="ProjectionMongoDBCopyCollectionName"/> from a <see cref="Type"/>.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to get the <see cref="ProjectionMongoDBCopyCollectionName"/> from.</param>
    /// <returns>The <see cref="ProjectionMongoDBCopyCollectionName"/> derived from the <see cref="CopyProjectionToMongoDBAttribute"/> or the <see cref="Type.Name"/>.</returns>
    public static ProjectionMongoDBCopyCollectionName GetFrom(Type type)
    {
        if (!type.TryGetDecorator<CopyProjectionToMongoDBAttribute>(out var decorator) || string.IsNullOrEmpty(decorator.CollectionName))
        {
            return type.Name;
        }
        return decorator.CollectionName;
    }
}
