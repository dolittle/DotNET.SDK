// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Common;
using Dolittle.SDK.Common.ClientSetup;

namespace Dolittle.SDK.Projections.Copies.MongoDB;

/// <summary>
/// Represents an implementation of <see cref="ICanBuildCopyDefinitionFromReadModel"/> that can build the <see cref="ProjectionCopyToMongoDB"/> definition.
/// </summary>
public class MongoDbCopyDefinitionFromReadModelBuilder : ICanBuildCopyDefinitionFromReadModel
{
    /// <inheritdoc />
    public bool CanBuildFrom<TReadModel>()
        where TReadModel : class, new()
        => typeof(TReadModel).TryGetDecorator<CopyProjectionToMongoDBAttribute>(out _);

    /// <inheritdoc />
    public bool BuildFrom<TReadModel>(IClientBuildResults buildResults, IProjectionCopyDefinitionBuilder<TReadModel> builder)
        where TReadModel : class, new()
    {
        var succeeded = true;
        if (!ProjectionMongoDBCopyCollectionName.TryGetFrom<TReadModel>(out var collectionName))
        {
            buildResults.AddFailure($"Could not get projection MongoDB Copy collection name from projection read model type {nameof(TReadModel)}");
            succeeded = false;
        }

        if (!succeeded)
        {
            return false;
        }

        builder.CopyToMongoDB(_ => _
            .ToCollection(collectionName));

        return true;
    }
}
