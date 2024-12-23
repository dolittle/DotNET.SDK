// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Generic;

namespace Dolittle.SDK.Events.Store;

/// <summary>
/// Represents a sequence of <see cref="UncommittedAggregateEvents"/>s that have not been committed to the Event Store.
/// </summary>
public class UncommittedAggregateEvents : IList<UncommittedAggregateEvent>
{
    readonly List<UncommittedAggregateEvent> _events = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="UncommittedAggregateEvents"/> class.
    /// </summary>
    /// <param name="eventSourceId">The Event Source that the uncommitted events was applied to.</param>
    /// <param name="aggregateRootId">The <see cref="AggregateRootId"/> of the aggregate that applied the events to the Event Source.</param>
    /// <param name="expectedAggregateRootVersion">The <see cref="AggregateRootVersion"/> of the Aggregate Root that was used to apply the rules that resulted in the Events.</param>
    public UncommittedAggregateEvents(EventSourceId eventSourceId, AggregateRootId aggregateRootId, AggregateRootVersion expectedAggregateRootVersion)
    {
        ThrowIfEventSourceIdIsNull(eventSourceId);
        ThrowIfAggregateRootIdIsNull(aggregateRootId);
        ThrowIfExpectedAggregateRootVersionIsNull(expectedAggregateRootVersion);

        EventSource = eventSourceId;
        AggregateRoot = aggregateRootId;
        ExpectedAggregateRootVersion = expectedAggregateRootVersion;
    }

    /// <summary>
    /// Gets the <see cref="EventSourceId" />.
    /// </summary>
    public EventSourceId EventSource { get; }

    /// <summary>
    /// Gets the <see cref="AggregateRootId" /> of the aggregate root that applied the events to the Event Source.
    /// </summary>
    public AggregateRootId AggregateRoot { get; }

    /// <summary>
    /// Gets the expected <see cref="AggregateRootVersion" />.
    /// </summary>
    public AggregateRootVersion ExpectedAggregateRootVersion { get; }

    /// <summary>
    /// Gets a value indicating whether or not there are any events in the committed sequence.
    /// </summary>
    public bool HasEvents => Count > 0;

    /// <inheritdoc/>
    public int Count => _events.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => false;

    /// <inheritdoc/>
    public UncommittedAggregateEvent this[int index]
    {
        get => _events[index];
        set => Insert(index, value);
    }

    /// <inheritdoc/>
    public IEnumerator<UncommittedAggregateEvent> GetEnumerator() => _events.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => _events.GetEnumerator();

    /// <inheritdoc/>
    public int IndexOf(UncommittedAggregateEvent item) => _events.IndexOf(item);

    /// <inheritdoc/>
    public void Insert(int index, UncommittedAggregateEvent item)
    {
        ThrowIfEventIsNull(item);
        _events.Insert(index, item);
    }

    /// <inheritdoc/>
    public void RemoveAt(int index) => _events.RemoveAt(index);

    /// <inheritdoc/>
    public void Add(UncommittedAggregateEvent item)
    {
        ThrowIfEventIsNull(item);
        _events.Add(item);
    }

    /// <inheritdoc/>
    public void Clear() => _events.Clear();

    /// <inheritdoc/>
    public bool Contains(UncommittedAggregateEvent item) => _events.Contains(item);

    /// <inheritdoc/>
    public void CopyTo(UncommittedAggregateEvent[] array, int arrayIndex)
    {
        foreach (var item in array)
        {
            ThrowIfEventIsNull(item);
        }

        _events.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public bool Remove(UncommittedAggregateEvent item) => _events.Remove(item);

    void ThrowIfEventSourceIdIsNull(EventSourceId eventSourceId)
    {
        if (eventSourceId == null) throw new EventSourceIdCannotBeNull();
    }

    void ThrowIfAggregateRootIdIsNull(AggregateRootId aggregateRootId)
    {
        if (aggregateRootId == null) throw new AggregateRootIdCannotBeNull();
    }

    void ThrowIfExpectedAggregateRootVersionIsNull(AggregateRootVersion expectedAggregateRootVersion)
    {
        if (expectedAggregateRootVersion == null) throw new ExpectedAggregateRootVersionCannotBeNull();
    }

    void ThrowIfEventIsNull(UncommittedAggregateEvent @event)
    {
        if (@event == null) throw new EventCannotBeNull();
    }
}
