// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Projections.Copies;

/// <summary>
/// Defines a factory for <see cref="IProjectionCopyDefinitionBuilder{TReadModel}"/>.
/// </summary>
public interface IProjectionCopyDefinitionBuilderFactory
{
    /// <summary>
    /// Creates a <see cref="IProjectionCopyDefinitionBuilder{TReadModel}"/>.
    /// </summary>
    /// <typeparam name="TReadModel">The <see cref="Type"/> of the projection read model.</typeparam>
    /// <returns>The created <see cref="IProjectionCopyDefinitionBuilder{TReadModel}"/>.</returns>
    IProjectionCopyDefinitionBuilder<TReadModel> CreateFor<TReadModel>() 
        where TReadModel : class, new();
}
