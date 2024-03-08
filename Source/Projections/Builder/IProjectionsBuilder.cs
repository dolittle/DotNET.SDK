// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;

namespace Dolittle.SDK.Projections.Builder;

/// <summary>
/// Defines a builder for building projections.
/// </summary>
public interface IProjectionsBuilder
{
    /// <summary>
    /// Start building an projection.
    /// </summary>
    /// <param name="projectionId">The <see cref="ProjectionId" />.</param>
    /// <returns>The builder for continuation.</returns>
    IProjectionBuilder Create(ProjectionId projectionId);
    
    /// <summary>
    /// Registers a <see cref="Type" /> as a projection class.
    /// </summary>
    /// <typeparam name="TProjection">The <see cref="Type" /> that is the projection class.</typeparam>
    /// <returns>The builder for continuation.</returns>
    IProjectionsBuilder Register<TProjection>()
        where TProjection : ReadModel, new();

    /// <summary>
    /// Registers a <see cref="Type" /> as a projection class.
    /// </summary>
    /// <param name="type">The <see cref="Type" /> of the projection.</param>
    /// <returns>The builder for continuation.</returns>
    IProjectionsBuilder Register(Type type);

    /// <summary>
    /// Registers all projection classes from an <see cref="Assembly" />.
    /// </summary>
    /// <param name="assembly">The <see cref="Assembly" /> to register the projection classes from.</param>
    /// <returns>The builder for continuation.</returns>
    IProjectionsBuilder RegisterAllFrom(Assembly assembly);
}
