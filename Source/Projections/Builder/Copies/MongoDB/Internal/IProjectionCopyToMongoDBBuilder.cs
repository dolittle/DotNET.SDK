// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Projections.Copies.MongoDB;

namespace Dolittle.SDK.Projections.Builder.Copies.MongoDB.Internal;

/// <summary>
/// Defines a system for building <see cref="ProjectionCopyToMongoDB"/>.
/// </summary>
/// <typeparam name="TReadModel">The <see cref="Type"/> of the projection read model.</typeparam>
public interface IProjectionCopyToMongoDBBuilder<TReadModel> : MongoDB.IProjectionCopyToMongoDBBuilder<TReadModel>
    where TReadModel : class, new()
{
    /// <summary>
    /// Gets the <see cref="IPropertyConversions"/>.
    /// </summary>
    IPropertyConversions Conversions { get; }

    /// <summary>
    /// Builds the <see cref="ProjectionCopyToMongoDB"/>.
    /// </summary>
    /// <param name="buildResults">The <see cref="IClientBuildResults"/>.</param>
    /// <param name="copyDefinition">The built <see cref="ProjectionCopyToMongoDB"/>.</param>
    /// <returns>True if successfully built.</returns>
    bool TryBuild(IClientBuildResults buildResults, out ProjectionCopyToMongoDB copyDefinition);
    
    /// <summary>
    /// Builds the <see cref="ProjectionCopyToMongoDB"/>.
    /// </summary>
    /// <param name="buildResults">The <see cref="IClientBuildResults"/>.</param>
    /// <param name="copyDefinition">The built <see cref="ProjectionCopyToMongoDB"/>.</param>
    /// <returns>True if successfully built.</returns>
    bool TryBuildFromReadModel(IClientBuildResults buildResults, out ProjectionCopyToMongoDB copyDefinition);


}
