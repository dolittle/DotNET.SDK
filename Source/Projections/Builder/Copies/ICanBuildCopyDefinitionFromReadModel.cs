// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Projections.Copies;

namespace Dolittle.SDK.Projections.Builder.Copies;

/// <summary>
/// Defines a system that can build a copy definition part of the <see cref="ProjectionCopies"/> from a projection read model <see cref="Type"/>. 
/// </summary>
public interface ICanBuildCopyDefinitionFromReadModel
{
    /// <summary>
    /// Gets whether this can build a copy definition from the given <typeparamref namef="TReadModel"/>.
    /// </summary>
    /// <typeparam name="TReadModel">The <see cref="Type"/> of the projection read model.</typeparam>
    /// <returns>True if the copy definition can be built from <typeparamref name="TReadModel"/>, false if not.</returns>
    bool CanBuildFrom<TReadModel>()
        where TReadModel : class, new();

    /// <summary>
    /// Builds the copy definition from the <typeparamref name="TReadModel"/> using the given <see cref="IProjectionCopyDefinitionBuilder{TReadModel}"/>.
    /// </summary>
    /// The <see cref="IClientBuildResults"/>.
    /// The <see cref="IProjectionCopyDefinitionBuilder{TReadModel}"/>.
    /// <typeparam name="TReadModel">The <see cref="Type"/> of the projection read model.</typeparam>
    /// <returns>True if successfully built copy definition, false if not.</returns>
    bool BuildFrom<TReadModel>(IClientBuildResults buildResults, IProjectionCopyDefinitionBuilder<TReadModel> builder)
        where TReadModel : class, new();
}
