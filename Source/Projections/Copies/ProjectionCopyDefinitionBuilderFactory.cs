// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Projections.Copies;

/// <summary>
/// Represents an implementation of <see cref="IProjectionCopyDefinitionBuilderFactory"/>.
/// </summary>
public class ProjectionCopyDefinitionBuilderFactory : IProjectionCopyDefinitionBuilderFactory
{
    /// <inheritdoc />
    public IProjectionCopyDefinitionBuilder<TReadModel> CreateFor<TReadModel>()
        where TReadModel : class, new()
        => throw new NotImplementedException();
}
