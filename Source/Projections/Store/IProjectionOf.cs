// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Projections.Store;

/// <summary>
/// Defines a system that knows about a projection.
/// </summary>
/// <typeparam name="TReadModel">The <see cref="Type" /> of the projection read model.</typeparam>
public interface IProjectionOf<TReadModel> where TReadModel : ReadModel, new()
{
    /// <summary>
    /// Gets the <see cref="ProjectionId"/> identifier.
    /// </summary>
    ProjectionId Identifier { get; }

    /// <summary>
    /// Gets the <see cref="ScopeId"/>.
    /// </summary>
    ScopeId Scope { get; }

    /// <summary>
    /// Gets the projection read model by key.
    /// </summary>
    /// <param name="key">The <see cref="Key" /> of the projection.</param>
    /// <param name="cancellation">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task" /> that, when resolved, returns the <typeparamref name="TReadModel"/> read model.</returns>
    Task<TReadModel?> Get(Key key, CancellationToken cancellation = default);
    
    IQueryable<TReadModel> AsQueryable();
}
