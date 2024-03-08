// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.SDK.Projections;

/// <summary>
/// Defines the different result types of <see cref="IProjection{TReadModel}.On(TReadModel, object, Events.EventType, Projections.ProjectionContext, System.Threading.CancellationToken)" />.
/// </summary>
public enum ProjectionResultType
{
    /// <summary>
    /// Replaces the read model.
    /// </summary>
    Replace = 0,

    /// <summary>
    /// Deletes the read model.
    /// </summary>
    Delete = 1,

    /// <summary>
    /// No change to the read model.
    /// </summary>
    Keep = 2
}
