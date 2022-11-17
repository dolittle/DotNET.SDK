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
    readonly MongoDB.Internal.IProjectionCopyToMongoDBBuilder<TReadModel> _copyToMongoDBBuilder;
    bool _copyToMongoDB;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionCopyDefinitionBuilder{TReadModel}"/> class.
    /// </summary>
    /// <param name="copyToMongoDBBuilder">The <see cref="MongoDB.Internal.IProjectionCopyToMongoDBBuilder{TReadModel}"/>.</param>
    public ProjectionCopyDefinitionBuilder(MongoDB.Internal.IProjectionCopyToMongoDBBuilder<TReadModel> copyToMongoDBBuilder)
    {
        _copyToMongoDBBuilder = copyToMongoDBBuilder;
    }

    /// <inheritdoc />
    public IProjectionCopyDefinitionBuilder<TReadModel> CopyToMongoDB(Action<IProjectionCopyToMongoDBBuilder<TReadModel>> callback = default)
    {
        _copyToMongoDB = true;
        callback?.Invoke(_copyToMongoDBBuilder);
        return this;
    }

    /// <inheritdoc />
    public bool TryBuild(ProjectionModelId identifier, IClientBuildResults buildResults, out ProjectionCopies definition)
    {
        definition = default;
        var succeeded = true;
        var mongoDBCopy = ProjectionCopyToMongoDB.Default;
        
        if (_copyToMongoDB && !_copyToMongoDBBuilder.TryBuild(identifier, buildResults, out mongoDBCopy))
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
