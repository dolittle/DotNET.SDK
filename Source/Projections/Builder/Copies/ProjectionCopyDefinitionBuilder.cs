// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Projections.Builder.Copies.MongoDB;
using Dolittle.SDK.Projections.Copies;
using Dolittle.SDK.Projections.Copies.MongoDB;

namespace Dolittle.SDK.Projections.Builder.Copies;

/// <summary>
/// Represents an implementation of <see cref="IProjectionCopyDefinitionBuilder{TReadModel}"/>.
/// </summary>
/// <typeparam name="TReadModel">The <see cref="Type"/> of the projection read model.</typeparam>
public class ProjectionCopyDefinitionBuilder<TReadModel> : IProjectionCopyDefinitionBuilder<TReadModel>
    where TReadModel : class, new()
{
    readonly ProjectionCopyToMongoDBBuilder<TReadModel> _mongoDbBuilder;
    bool _copyToMongoDB;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionCopyDefinitionBuilder{TReadModel}"/> class.
    /// </summary>
    /// <param name="mongoDBCOllectionNameValidator">The <see cref="IValidateMongoDBCollectionName"/>.</param>
    /// <param name="defaultBsonConversionGetter">The <see cref="IGetDefaultConversionsFromReadModel"/>.</param>
    public ProjectionCopyDefinitionBuilder(IValidateMongoDBCollectionName mongoDBCOllectionNameValidator, IGetDefaultConversionsFromReadModel defaultBsonConversionGetter)
    {
        _mongoDbBuilder = new ProjectionCopyToMongoDBBuilder<TReadModel>(mongoDBCOllectionNameValidator, defaultBsonConversionGetter);
    }

    /// <inheritdoc />
    public IProjectionCopyDefinitionBuilder<TReadModel> CopyToMongoDB(Action<IProjectionCopyToMongoDBBuilder<TReadModel>> callback)
    {
        _copyToMongoDB = true;
        callback(_mongoDbBuilder);
        return this;
    }

    /// <inheritdoc />
    public bool TryBuild(IClientBuildResults buildResults, out ProjectionCopies definition)
    {
        definition = default;
        var succeeded = true;
        var mongoDBCopy = ProjectionCopyToMongoDB.Default;
        
        if (_copyToMongoDB && !_mongoDbBuilder.TryBuild(buildResults, out mongoDBCopy))
        {
            buildResults.AddFailure($"Failed to build Copy To MongoDB Definition");
            succeeded = false;
        }
        if (!succeeded)
        {
            return false;
        }
        definition = new ProjectionCopies(mongoDBCopy);
        return true;
    }
}
