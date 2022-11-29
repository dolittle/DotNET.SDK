// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Common;

namespace Dolittle.SDK.Projections.Store;

/// <summary>
/// Defines a associations for projections.
/// </summary>
public interface IProjectionReadModelTypes : IUniqueBindings<ProjectionModelId, Type>
{
    /// <summary>
    /// Try get the <see cref="ProjectionId" /> associated with <typeparamref name="TProjection"/>.
    /// </summary>
    /// <typeparam name="TProjection">The <see cref="Type" /> of the projection.</typeparam>
    /// <returns>The <see cref="ProjectionModelId" />.</returns>
    ProjectionModelId GetFor<TProjection>()
        where TProjection : class, new();
}
