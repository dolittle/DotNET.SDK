// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Common;
using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Projections.Copies.MongoDB;

namespace Dolittle.SDK.Projections.Builder.Copies.MongoDB.Internal;

/// <summary>
/// Represents an implementation of <see cref="IMongoDBCopyDefinitionFromReadModelBuilder"/> that can build the <see cref="ProjectionCopyToMongoDB"/> definition.
/// </summary>
public class MongoDBCopyDefinitionFromReadModelBuilder : IMongoDBCopyDefinitionFromReadModelBuilder
{
    readonly IBuildPropertyConversionsFromMongoDBConvertToAttributes _conversionsFromAttributesBuilder;

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoDBCopyDefinitionFromReadModelBuilder"/> class.
    /// </summary>
    /// <param name="conversionsFromAttributesBuilder">The <see cref="IBuildPropertyConversionsFromMongoDBConvertToAttributes"/>.</param>
    public MongoDBCopyDefinitionFromReadModelBuilder(IBuildPropertyConversionsFromMongoDBConvertToAttributes conversionsFromAttributesBuilder)
    {
        _conversionsFromAttributesBuilder = conversionsFromAttributesBuilder;
    }
    
    /// <inheritdoc />
    public bool CanBuild<TReadModel>() where TReadModel : class, new()
        => typeof(TReadModel).TryGetDecorator<CopyProjectionToMongoDBAttribute>(out _);

    /// <inheritdoc />
    public bool TryBuild<TReadModel>(IClientBuildResults buildResults, IProjectionCopyToMongoDBBuilder<TReadModel> copyToMongoDbBuilder)
        where TReadModel : class, new()
    {
        if (!CanBuild<TReadModel>())
        {
            buildResults.AddFailure($"Could not get projection MongoDB Copy collection name from projection read model type {nameof(TReadModel)}");
            return false;
        }
        if (!_conversionsFromAttributesBuilder.TryBuildFrom<TReadModel>(buildResults, copyToMongoDbBuilder.Conversions))
        {
            buildResults.AddFailure($"Could not build MongoDB Copy property conversions from projection read model type {nameof(TReadModel)}");
            return false;
        }
        copyToMongoDbBuilder.ToCollection(ProjectionMongoDBCopyCollectionName.GetFrom<TReadModel>());

        return true;
    }
}
