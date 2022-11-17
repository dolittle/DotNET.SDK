// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Common;

namespace Dolittle.SDK.Projections.Store;

/// <summary>
/// Represents an implementation of <see cref="IProjectionReadModelTypes" />.
/// </summary>
public class ProjectionReadModelTypes : UniqueBindings<ProjectionModelId, Type>, IProjectionReadModelTypes
{
    /// <inheritdoc/>
    public ProjectionModelId GetFor<TProjection>()
        where TProjection : class, new()
        => base.GetFor(typeof(TProjection));

}
