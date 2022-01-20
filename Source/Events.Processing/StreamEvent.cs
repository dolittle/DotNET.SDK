// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events.Store;

namespace Dolittle.SDK.Events.Processing;

/// <summary>
/// Represents an Event from a Stream in the Event Store.
/// </summary>
public class StreamEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StreamEvent"/> class.
    /// </summary>
    /// <param name="event">The <see cref="CommittedEvent"/> in the Stream.</param>
    /// <param name="partitioned">Whether the Event comes from a partitioned Stream or not.</param>
    /// <param name="partition">The <see cref="PartitionId"/> assigned to the Event in the Stream.</param>
    /// <param name="scope">The <see cref="ScopeId"/> of the Stream.</param>
    public StreamEvent(CommittedEvent @event, bool partitioned, PartitionId partition, ScopeId scope)
    {
        Event = @event;
        Partitioned = partitioned;
        Partition = partition;
        Scope = scope;
    }

    /// <summary>
    /// Gets the committed Event.
    /// </summary>
    public CommittedEvent Event { get; }

    /// <summary>
    /// Gets a value indicating whether the Event comes from a partitioned Stream or not.
    /// </summary>
    public bool Partitioned { get; }

    /// <summary>
    /// Gets the partition if the Event comes from a partitioned Stream.
    /// </summary>
    /// <remarks>
    /// This value must not be used if <see cref="Partitioned"/> is not true.
    /// </remarks>
    public PartitionId Partition { get; }

    /// <summary>
    /// Gets the scope of the Stream the Event comes from.
    /// </summary>
    public ScopeId Scope { get; }
}