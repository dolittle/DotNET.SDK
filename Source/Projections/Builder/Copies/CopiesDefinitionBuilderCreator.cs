// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Projections.Copies.MongoDB;

namespace Dolittle.SDK.Projections.Builder.Copies;

/// <summary>
/// Represents an implementation of <see cref="ICreateCopiesDefinitionBuilder"/>.
/// </summary>
public class CopiesDefinitionBuilderCreator : ICreateCopiesDefinitionBuilder
{
    readonly IValidateMongoDBCollectionName _mongoDBCollectionNameValidator;
    readonly IGetDefaultConversionsFromReadModel _defaultBsonConversionsGetter;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="CopiesDefinitionBuilderCreator"/> class. 
    /// </summary>
    /// <param name="mongoDbCollectionNameValidator">The <see cref="IValidateMongoDBCollectionName"/>.</param>
    /// <param name="defaultBsonConversionsGetter">The <see cref="IGetDefaultConversionsFromReadModel"/>.</param>
    public CopiesDefinitionBuilderCreator(IValidateMongoDBCollectionName mongoDbCollectionNameValidator, IGetDefaultConversionsFromReadModel defaultBsonConversionsGetter)
    {
        _mongoDBCollectionNameValidator = mongoDbCollectionNameValidator;
        _defaultBsonConversionsGetter = defaultBsonConversionsGetter;
    }

    /// <inheritdoc />
    public IProjectionCopyDefinitionBuilder<TReadModel> CreateFor<TReadModel>()
        where TReadModel : class, new()
        => new ProjectionCopyDefinitionBuilder<TReadModel>(_mongoDBCollectionNameValidator, _defaultBsonConversionsGetter);
}
