// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Testing.Aggregates.Events;

/// <summary>
/// Fluent interface element allowing assertions against an event in the stream, chained to allow further assertions
/// against the specific event.
/// </summary>
/// <typeparam name="T">The type of the event to assert against.</typeparam>
public class EventSequenceAssertion<T>
    where T : class
{
    readonly IList<AppliedEvent> _allEvents;
    readonly OnDolittleAssertionFailed _throwError;
    readonly Func<AppliedEvent, bool> _isWantedEvent;
    readonly IList<T> _conformingEvents;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventSequenceAssertion{T}"/> class.
    /// </summary>
    /// <param name="sequence">The list of <see cref="AppliedEvent" /> to assert against.</param>
    /// <param name="isPublic">Whether the events to check </param>
    /// <param name="throwError">THe callback for throwing error</param>
    public EventSequenceAssertion(IList<AppliedEvent> sequence, bool isPublic, OnDolittleAssertionFailed? throwError = default)
        : this(sequence, evt => evt.Event is T && evt.Public == isPublic, throwError ?? DolittleAssertionFailed.Throw)
    {
    }
    
    EventSequenceAssertion(IList<AppliedEvent> sequence, Func<AppliedEvent, bool> isWantedEvent, OnDolittleAssertionFailed throwError)
    {
        _allEvents = sequence;
        _throwError = throwError;
        _isWantedEvent = isWantedEvent;
        _conformingEvents = sequence.Where(_isWantedEvent).Select(_ => (T)_.Event).ToList();
    }
    
    /// <summary>
    /// Asserts that an event of the specified type is present anywhere in the sequence, allowing further assertions against the first instance.
    /// </summary>
    /// <returns>An EventValueAssertion{T} to allow assertions against the event instance.</returns>
    public EventValueAssertion<T> First()
        => new(_conformingEvents.First());

    /// <summary>
    /// Asserts that an event of the specified type is present anywhere in the sequence, allowing further assertions against the last instance.
    /// </summary>
    /// <returns>An EventValueAssertion{T} to allow assertions against the event instance.</returns>
    public EventValueAssertion<T> Last()
        => new(_conformingEvents.Last());

    /// <summary>
    /// Asserts that the event of the specified type that is the given number exists, allowing further assertions against that instance.
    /// </summary>
    /// <returns>An EventValueAssertion{T} to allow assertions against the event instance.</returns>
    public EventValueAssertion<T> Number(int num)
    {
        var numConformingEvents = _conformingEvents.Count;
        if (numConformingEvents < num)
        {
            _throwError($"there are only {numConformingEvents} conforming events, not {num}");
        }
        return new EventValueAssertion<T>(_conformingEvents[num - 1]);
    }

    /// <summary>
    /// Asserts that there are a certain number of events in the sequence.
    /// </summary>
    /// <param name="expectedNum">THe expected number of events.</param>
    public EventSequenceAssertion<T> CountOf(int expectedNum)
    {
        var numConformingEvents = _conformingEvents.Count;
        if (numConformingEvents != expectedNum)
        {
            _throwError($"there are {numConformingEvents} conforming events, expected {expectedNum}");
        }
        return this;
    }
    
    /// <summary>
    /// Asserts that an event of the specified type is the first event in the sequence, allowing further assertions against the instance.
    /// </summary>
    /// <returns>An EventValueAssertion{T} to allow assertions against the event instance.</returns>
    public EventValueAssertion<T> AtBeginning()
        => AtSequenceNumber(0);

    /// <summary>
    /// Asserts that an event of the specified type is the last event in the sequence, allowing further assertions against the instance.
    /// </summary>
    /// <returns>An EventValueAssertion{T} to allow assertions against the event instance.</returns>
    public EventValueAssertion<T> AtEnd()
        => AtSequenceNumber(_allEvents.Count - 1);

    /// <summary>
    /// Asserts that an event of the specified type is present at the specified index of the current event sequence of the aggregate, allowing further assertions against the instance.
    /// </summary>
    /// <param name="sequenceNumber">Position in the event sequence.</param>
    /// <returns>An EventValueAssertion{T} to allow assertions against the event instance.</returns>
    public EventValueAssertion<T> AtSequenceNumber(int sequenceNumber)
    {
        if (sequenceNumber > _allEvents.Count)
        {
            _throwError($"there cannot be an event at sequence number {sequenceNumber} of aggregate as it is greater than the number of events {_allEvents.Count}");
        }
        var evt = _allEvents[sequenceNumber];
        if (!_isWantedEvent(evt))
        {
            _throwError($"event at sequence number {sequenceNumber} of aggregate is not one of the wanted events.");
        }
        return new EventValueAssertion<T>((T)evt.Event);
    }

    /// <summary>
    /// Asserts that each event of the specified type conforms to the given predicate. 
    /// </summary>
    /// <param name="predicate">The predicate that each event should conform to.</param>
    public void WhereAll(params Action<T>[] assertions)
    {
        foreach (var @event in _conformingEvents)
        {
            foreach (var assertion in assertions)
            {
                assertion(@event);
            }
        }
    }

    /// <summary>
    /// Asserts that each event of the specified type conforms to the given predicate. 
    /// </summary>
    /// <param name="predicate">The predicate that each event should conform to.</param>
    public EventSequenceAssertion<T> Where(Func<T, bool> predicate)
        => new(_allEvents, _ => _isWantedEvent(_) && predicate((T) _.Event), _throwError);
}
