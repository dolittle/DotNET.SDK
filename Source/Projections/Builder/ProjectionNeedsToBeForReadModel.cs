// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Projections.Builder;

/// <summary>
/// Exception that gets thrown when no read model is specified while creating a projection.
/// </summary>
public class ProjectionNeedsToBeForReadModel : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionNeedsToBeForReadModel"/> class.
    /// </summary>
    /// <param name="projectionId">The <see cref="ProjectionId" />.</param>
    public ProjectionNeedsToBeForReadModel(ProjectionId projectionId)
        : base($"Projection {projectionId} could not be built because {nameof(ProjectionBuilder.ForReadModel)} needs to be called when creating a projection")
    {
    }
}