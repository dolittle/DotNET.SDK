// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;

namespace Dolittle.SDK.Events.Filters
{
    /// <summary>
    /// Filters an event.
    /// </summary>
    /// <param name="event">The event.</param>
    /// <param name="eventContext">The <see cref="EventContext" />.</param>
    public delegate Task<bool> FilterEventCallback(object @event, EventContext eventContext);

    /// <summary>
    /// Filters an event to a partition.
    /// </summary>
    /// <param name="event">The event.</param>
    /// <param name="eventContext">The <see cref="EventContext" />.</param>
    public delegate Task<bool> PartitionedFilterEventCallback(object @event, EventContext eventContext);

    /// <summary>
    /// Filters an event.
    /// </summary>
    /// <param name="event">The event.</param>
    /// <param name="eventContext">The <see cref="EventContext" />.</param>
    public delegate Task<bool> FilterEventCallback<T>(T @event, EventContext eventContext)
        where T : class;

    /// <summary>
    /// Filters an event to a partition.
    /// </summary>
    /// <param name="event">The event.</param>
    /// <param name="eventContext">The <see cref="EventContext" />.</param>
    public delegate Task<bool> PartitionedFilterEventCallback<T>(T @event, EventContext eventContext)
        where T : class;
}