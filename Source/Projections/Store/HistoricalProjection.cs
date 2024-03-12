// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Store;

namespace Dolittle.SDK.Projections.Store;

public class HistoricalProjection<TProjection> : IHistoricalProjection<TProjection>
    where TProjection : ReadModel, new()
{
    readonly IProjection<TProjection> _projection;
    readonly IEventStore _eventStore;

    public HistoricalProjection(IProjection<TProjection> projection, IEventStore eventStore)
    {
        if (!projection.SupportsHistoricalQueries) throw new ArgumentException("The projection does not support historical queries", nameof(projection));
        _projection = projection;
        _eventStore = eventStore;
    }

    public Task<TProjection?> GetAt(Key key, DateTimeOffset until, CancellationToken cancellation = default) => throw new NotImplementedException();

    public IAsyncEnumerable<TProjection> GetAll<TCloneableProjection>(Key key, DateTimeOffset? until, CancellationToken cancellation = default)
        where TCloneableProjection : TProjection, ICloneable => throw new NotImplementedException();

    public IAsyncEnumerable<(ProjectionResult<TProjection> result, EventContext context)> GetDetailedResults<TCloneableProjection>(Key key,
        DateTimeOffset? until, CancellationToken cancellation = default) where TCloneableProjection : TProjection, ICloneable =>
        throw new NotImplementedException();

    IAsyncEnumerable<CommittedEvent> GetEvents(Key key, DateTimeOffset? until, CancellationToken cancellation = default)
    {
        
    }
}
