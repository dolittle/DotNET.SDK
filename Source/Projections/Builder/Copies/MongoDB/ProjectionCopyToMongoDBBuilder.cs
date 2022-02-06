// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Projections.Builder.Copies.MongoDB.Internal;
using Dolittle.SDK.Projections.Copies;
using Dolittle.SDK.Projections.Copies.MongoDB;

namespace Dolittle.SDK.Projections.Builder.Copies.MongoDB;


/// <summary>
/// Represents an implementation of <see cref="IProjectionCopyToMongoDBBuilder{TReadModel}"/>.
/// </summary>
/// <typeparam name="TReadModel">The <see cref="Type"/> of the projection read model.</typeparam>
public class ProjectionCopyToMongoDBBuilder<TReadModel> : Internal.IProjectionCopyToMongoDBBuilder<TReadModel>
    where TReadModel : class, new()
{
    readonly IValidateMongoDBCollectionName _collectionNameValidator;
    readonly IBuildPropertyConversionsFromBsonClassMap _conversionsFromBSONClassMap;
    readonly IMongoDBCopyDefinitionFromReadModelBuilder _mongoDbCopyFromReadModelBuilder;
    readonly IResolvePropertyPath _propertyPathResolver;
    MongoDBCopyCollectionName _collectionName;
    bool _withoutDefaultConversions;
    Dictionary<PropertyPath, Conversion> _explicitConversions = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionCopyToMongoDBBuilder{TReadMOdel}"/> class.
    /// </summary>
    /// <param name="collectionNameValidator">The <see cref="IValidateMongoDBCollectionName"/>.</param>
    /// <param name="conversionsFromBSONClassMap">The <see cref="IBuildPropertyConversionsFromBsonClassMap"/>.</param>
    /// <param name="mongoDBCopyFromReadModelBuilder">The <see cref="IMongoDBCopyDefinitionFromReadModelBuilder"/>.</param>
    /// <param name="propertyPathResolver">The <see cref="IResolvePropertyPath"/>.</param>
    public ProjectionCopyToMongoDBBuilder(
        IValidateMongoDBCollectionName collectionNameValidator,
        IBuildPropertyConversionsFromBsonClassMap conversionsFromBSONClassMap,
        IMongoDBCopyDefinitionFromReadModelBuilder mongoDBCopyFromReadModelBuilder,
        IResolvePropertyPath propertyPathResolver)
    {
        Conversions = new PropertyConversions();
        _collectionNameValidator = collectionNameValidator;
        _conversionsFromBSONClassMap = conversionsFromBSONClassMap;
        _mongoDbCopyFromReadModelBuilder = mongoDBCopyFromReadModelBuilder;
        _propertyPathResolver = propertyPathResolver;
        _collectionName = MongoDBCopyCollectionName.GetFrom<TReadModel>();
    }

    /// <inheritdoc />
    public IPropertyConversions Conversions { get; }

    /// <inheritdoc />
    public IProjectionCopyToMongoDBBuilder<TReadModel> ToCollection(MongoDBCopyCollectionName name)
    {
        _collectionName = name;
        return this;
    }

    /// <inheritdoc />
    public IProjectionCopyToMongoDBBuilder<TReadModel> WithConversion<TProperty>(Expression<Func<TReadModel, TProperty>> fieldExpression, Conversion conversion)
    {
        _explicitConversions[_propertyPathResolver.FromExpression(fieldExpression)] = conversion;
        return this;
    }

    /// <inheritdoc />
    public IProjectionCopyToMongoDBBuilder<TReadModel> WithoutDefaultConversions()
    {
        _withoutDefaultConversions = true;
        return this;
    }

    /// <inheritdoc />
    public bool TryBuild(IClientBuildResults buildResults, out ProjectionCopyToMongoDB copyDefinition)
    {
        copyDefinition = default;
        var succeeded = true;

        if (!_collectionNameValidator.Validate(buildResults, _collectionName))
        {
            buildResults.AddFailure($"MongoDB Copy collection name {_collectionName} is not valid");
            succeeded = false;
        }
        if (!_withoutDefaultConversions && !_conversionsFromBSONClassMap.TryBuildFrom<TReadModel>(buildResults, Conversions))
        {
            buildResults.AddFailure($"MongoDB Copy failed getting default conversions based on BSON Class Map of the read model type");
            succeeded = false;
        }
        if (!succeeded)
        {
            return false;
        }
        foreach (var (path, conversion) in _explicitConversions)
        {
            Conversions.AddConversion(path, conversion);
        }
        copyDefinition = new ProjectionCopyToMongoDB(true, _collectionName, Conversions.GetAll());
        return true;
    }

    /// <inheritdoc />
    public bool TryBuildFromReadModel(IClientBuildResults buildResults, out ProjectionCopyToMongoDB copyDefinition)
    {
        copyDefinition = default;
        if (_mongoDbCopyFromReadModelBuilder.CanBuild<TReadModel>())
        {
            return _mongoDbCopyFromReadModelBuilder.TryBuild(buildResults, this) && TryBuild(buildResults, out copyDefinition);
        }
        copyDefinition = ProjectionCopyToMongoDB.Default;
        return true;
    }
}
