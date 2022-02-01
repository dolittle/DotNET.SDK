// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Dolittle.SDK.Common.ClientSetup;
using MongoDB.Bson;

namespace Dolittle.SDK.Projections.Copies.MongoDB;

/// <summary>
/// Represents an implementation of <see cref="IProjectionCopyToMongoDBBuilder{TReadModel}"/>.
/// </summary>
/// <typeparam name="TReadModel">The <see cref="Type"/> of the projection read model.</typeparam>
public class ProjectionCopyToMongoDBBuilder<TReadModel> : IProjectionCopyToMongoDBBuilder<TReadModel>
    where TReadModel : class, new()
{
    readonly IValidateMongoDBCollectionName _collectionNameValidator;
    readonly IGetDefaultConversionsFromReadModel _conversionsResolver;
    readonly Dictionary<string, BsonType> _conversions = new();
    ProjectionMongoDBCopyCollectionName _name;
    bool _withoutDefaultConversions;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionCopyToMongoDBBuilder{TReadMOdel}"/> class.
    /// </summary>
    /// <param name="collectionNameValidator">The <see cref="IValidateMongoDBCollectionName"/>.</param>
    /// <param name="conversionsResolver">The <see cref="IGetDefaultConversionsFromReadModel"/>.</param>
    public ProjectionCopyToMongoDBBuilder(IValidateMongoDBCollectionName collectionNameValidator, IGetDefaultConversionsFromReadModel conversionsResolver)
    {
        _collectionNameValidator = collectionNameValidator;
        _conversionsResolver = conversionsResolver;
    }

    /// <inheritdoc />
    public IProjectionCopyToMongoDBBuilder<TReadModel> ToCollection(ProjectionMongoDBCopyCollectionName name)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public IProjectionCopyToMongoDBBuilder<TReadModel> WithConversion(string field, BsonType bsonType)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public IProjectionCopyToMongoDBBuilder<TReadModel> WithConversion(Expression<Func<TReadModel, object>> fieldExpression, BsonType bsonType)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public IProjectionCopyToMongoDBBuilder<TReadModel> WithoutDefaultConversions()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Builds the <see cref="ProjectionCopyToMongoDB"/>.
    /// </summary>
    /// <param name="buildResults">The <see cref="IClientBuildResults"/>.</param>
    /// <param name="copyDefinition">The built <see cref="ProjectionCopyToMongoDB"/>.</param>
    /// <returns>True if successfully built.</returns>
    public bool TryBuild(IClientBuildResults buildResults, out ProjectionCopyToMongoDB copyDefinition)
    {
        // if (collectionName != default && !_collectionNameValidator.Validate(buildResults, collectionName))
        // {
        //     buildResults.AddFailure($"MongoDB Copy collection name {collectionName} from projection read model type {nameof(TReadModel)} is not valid");
        //     succeeded = false;
        // }
        // if (!_conversionsResolver.TryGetFrom<TReadModel>(buildResults, out var conversions))
        // {
        //     buildResults.AddFailure($"Could not get projection MongoDB Copy conversions from projection read model type {nameof(TReadModel)}");
        //     succeeded = false;
        // }
        throw new NotImplementedException();
    }
}
