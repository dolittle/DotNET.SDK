// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.SDK.Projections;

/// <summary>
/// Represents the base class for a read model.
/// </summary>
public abstract class ProjectionBase
{
    /// <summary>
    /// Gets or sets the unique identifier for the read model entity.
    /// </summary>
    public string Id { get; init; }
}
