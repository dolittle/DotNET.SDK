// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Projections.Store;

/// <summary>
/// Historical queries for projections.
/// These will allow you to query the state of a projection at a specific point in time.
/// Since these need to replay events, they are inherently slower and more resource intensive than the regular queries.
/// These will also only work for projections that are only using events keyed on EventSourceId (default behavior).
/// </summary>
/// <typeparam name="TProjection"></typeparam>
public interface IHistoricalProjection<TProjection> where TProjection : ReadModel, new()
{
    /// <summary>
    /// Get the state of the key at a specific point in time.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="until"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    public Task<TProjection?> GetAt(Key key, DateTimeOffset until, CancellationToken cancellation = default);

    /// <summary>
    /// Get the states of the key up to a specific point in time.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="until"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    public IAsyncEnumerable<TProjection> GetAll<TCloneableProjection>(Key key, DateTimeOffset? until, CancellationToken cancellation = default)
        where TCloneableProjection : TProjection, ICloneable;

    public IAsyncEnumerable<(ProjectionResult<TProjection> result, EventContext context)> GetDetailedResults<TCloneableProjection>(Key key,
        DateTimeOffset? until,
        CancellationToken cancellation = default) where TCloneableProjection : TProjection, ICloneable;
}
