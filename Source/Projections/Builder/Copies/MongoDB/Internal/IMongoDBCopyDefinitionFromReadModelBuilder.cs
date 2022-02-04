// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Common.ClientSetup;

namespace Dolittle.SDK.Projections.Builder.Copies.MongoDB.Internal;

/// <summary>
/// Defines a system that builds a <see cref="MongoDBCopyDefinitionFromReadModelBuilder{TReadModel}"/> from a projection read model <see cref="Type"/>. 
/// </summary>
public interface IMongoDBCopyDefinitionFromReadModelBuilder
{
    /// <summary>
    /// Checks whether a <see cref="Projections.Copies.MongoDB.ProjectionCopyToMongoDB"/> can be built from the <typeparamref name="TReadModel"/>.
    /// </summary>
    /// <typeparam name="TReadModel">The <see cref="Type"/> of the projection read model.</typeparam>
    /// <returns>True if can be built, false if not.</returns>
    bool CanBuild<TReadModel>()
        where TReadModel : class, new();
    /// <summary>
    /// Builds the copy definition from the <typeparamref name="TReadModel"/> using the given <see cref="IProjectionCopyDefinitionBuilder{TReadModel}"/>.
    /// </summary>
    /// <typeparam name="TReadModel">The <see cref="Type"/> of the projection read model.</typeparam>
    /// <returns>True if successfully built copy definition, false if not.</returns>
    bool TryBuild<TReadModel>(IClientBuildResults buildResults, IProjectionCopyToMongoDBBuilder<TReadModel> copyToMongoDBBuilder)
        where TReadModel : class, new();
}
