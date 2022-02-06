// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq.Expressions;
using Dolittle.SDK.Projections.Copies.MongoDB;
using MongoDB.Bson;

namespace Dolittle.SDK.Projections.Builder.Copies.MongoDB;

/// <summary>
/// Defines a system for building <see cref="ProjectionCopyToMongoDB"/>.
/// </summary>
/// <typeparam name="TReadModel">The <see cref="Type"/> of the projection read model.</typeparam>
public interface IProjectionCopyToMongoDBBuilder<TReadModel>
    where TReadModel : class, new()
{
    /// <summary>
    /// Sets the <see cref="ProjectionCopyToMongoDB"/>.
    /// </summary>
    /// <param name="name">The <see cref="ProjectionCopyToMongoDB"/>.</param>
    /// <returns>The builder for continuation</returns>
    IProjectionCopyToMongoDBBuilder<TReadModel> ToCollection(MongoDBCopyCollectionName name);

    /// <summary>
    /// Sets the conversion from a field to a <see cref="BsonType"/>.
    /// </summary>
    /// <param name="fieldExpression">The <see cref="Expression{TDelegate}"/> for getting the name of the field.</param>
    /// <param name="conversion">The <see cref="Conversion" />.</param>
    /// <returns>The builder for continuation</returns>
    IProjectionCopyToMongoDBBuilder<TReadModel> WithConversion<TProperty>(Expression<Func<TReadModel, TProperty>> fieldExpression, Conversion conversion);
    
    /// <summary>
    /// Signifies to the builder that it should not use the default BsonClassMap for the conversions of the fields.
    /// </summary>
    /// <returns>The builder for continuation</returns>
    IProjectionCopyToMongoDBBuilder<TReadModel> WithoutDefaultConversions();
}
