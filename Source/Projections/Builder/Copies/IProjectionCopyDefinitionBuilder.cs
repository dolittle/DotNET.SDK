// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.ApplicationModel.ClientSetup;
using Dolittle.SDK.Projections.Builder.Copies.MongoDB;
using Dolittle.SDK.Projections.Copies;
using Dolittle.SDK.Projections.Copies.MongoDB;

namespace Dolittle.SDK.Projections.Builder.Copies;

/// <summary>
/// Defines a system that can build <see cref="ProjectionCopies"/>.
/// </summary>
/// <typeparam name="TReadModel">The <see cref="Type"/> of the projection read model.</typeparam>
public interface IProjectionCopyDefinitionBuilder<TReadModel>
    where TReadModel : class, new()
{
    /// <summary>
    /// Build on the <see cref="ProjectionCopies"/> using the <see cref="IProjectionCopyToMongoDBBuilder{TReadModel}"/>.
    /// </summary>
    /// <param name="callback">The optional callback for building the <see cref="ProjectionCopyToMongoDB"/>.</param>
    /// <returns>The builder for continuation.</returns>
    IProjectionCopyDefinitionBuilder<TReadModel> CopyToMongoDB(Action<IProjectionCopyToMongoDBBuilder<TReadModel>> callback = default);

    /// <summary>
    /// Builds the <see cref="ProjectionCopies"/>.
    /// </summary>
    /// <param name="buildResults">The <see cref="IClientBuildResults"/>.</param>
    /// <param name="definition">The built <see cref="ProjectionCopies"/>.</param>
    /// <returns>True if successfully built, false if not.</returns>
    bool TryBuild(IClientBuildResults buildResults, out ProjectionCopies definition);
}
