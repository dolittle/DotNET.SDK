// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Projections.Copies.MongoDB;

namespace Dolittle.SDK.Projections.Builder.Copies.MongoDB.Internal;

/// <summary>
/// Represents an implementation of <see cref="IProjectionCopyToMongoDBBuilderFactory"/>.
/// </summary>
public class ProjectionCopyToMongoDbBuilderFactory : IProjectionCopyToMongoDBBuilderFactory
{
    readonly IValidateMongoDBCollectionName _collectionNameValidator;
    readonly IPropertyConversionsBuilder _propertyConversionsBuilder;
    readonly IBuildPropertyConversionsFromBsonClassMap _conversionsFromBsonClassMapBuilder;
    readonly IBuildPropertyConversionsFromMongoDBConvertToAttributes _conversionsFromAttributesBuilder;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionCopyToMongoDbBuilderFactory"/> class.
    /// </summary>
    /// <param name="collectionNameValidator"></param>
    /// <param name="propertyConversionsBuilder"></param>
    /// <param name="conversionsFromBsonClassMapBuilder"></param>
    /// <param name="conversionsFromAttributesBuilder"></param>
    public ProjectionCopyToMongoDbBuilderFactory(
        IValidateMongoDBCollectionName collectionNameValidator,
        IPropertyConversionsBuilder propertyConversionsBuilder,
        IBuildPropertyConversionsFromBsonClassMap conversionsFromBsonClassMapBuilder,
        IBuildPropertyConversionsFromMongoDBConvertToAttributes conversionsFromAttributesBuilder)
    {
        _collectionNameValidator = collectionNameValidator;
        _propertyConversionsBuilder = propertyConversionsBuilder;
        _conversionsFromBsonClassMapBuilder = conversionsFromBsonClassMapBuilder;
        _conversionsFromAttributesBuilder = conversionsFromAttributesBuilder;
    }
    
    /// <inheritdoc />
    public IProjectionCopyToMongoDBBuilder<TReadModel> CreateFor<TReadModel>() where TReadModel : class, new()
        => new ProjectionCopyToMongoDBBuilder<TReadModel>(_collectionNameValidator, _propertyConversionsBuilder, _conversionsFromBsonClassMapBuilder, new MongoDBCopyDefinitionFromReadModelBuilder(_conversionsFromAttributesBuilder));
}
