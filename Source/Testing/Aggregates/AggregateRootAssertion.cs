﻿// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Aggregates;
using System;
using System.Linq;
using Dolittle.SDK.Testing.Aggregates.Events;
using Dolittle.SDK.Testing.Assertion;

namespace Dolittle.SDK.Testing.Aggregates;

/// <summary>
/// Represents the assertions that can be performed on an <see cref="AggregateRoot"/>.
/// </summary>
public class AggregateRootAssertion<TAggregate>
    where TAggregate : AggregateRoot
{
    readonly TAggregate _aggregate;
    readonly AppliedEvent[] _events;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateRootAssertion{TAggregate}"/> class.
    /// </summary>
    /// <param name="aggregate">The <see cref="AggregateRoot"/>.</param>
    /// <param name="eventsToSkip">The optional number of applied events to disregard.</param>
    public AggregateRootAssertion(TAggregate aggregate, int eventsToSkip = 0)
    {
        _aggregate = aggregate;
        _events = aggregate.AppliedEvents.Skip(eventsToSkip).ToArray();
    }
    
    void Throw(string reason) => AggregateAssertionFailed.Throw(_aggregate, reason);

    /// <summary>
    /// Implicitly converts <see cref="AggregateRoot"/> to <see cref="AggregateRootAssertion{TAggregate}"/>.
    /// </summary>
    /// <param name="aggregate">The <see cref="AggregateRoot"/> to convert.</param>
    /// <returns>The <see cref="AggregateRootAssertion{TAggregate}"/>.</returns>
    public static implicit operator AggregateRootAssertion<TAggregate>(TAggregate aggregate)
        => new(aggregate);
    
    /// <summary>
    /// Starts the Fluent Interface by establishing an Event sequence to assert against.
    /// </summary>
    /// <typeparam name="T"><see cref="Type" /> of the event that you wish to assert against.</typeparam>
    /// <returns>An <see cref="EventSequenceAssertion{T}" /> scoped to your event type.</returns>
    public EventSequenceAssertion<T> ShouldHaveEvent<T>()
        where T : class
        => ShouldHaveEvent<T>(false);

    /// <summary>
    /// Starts the Fluent Interface by establishing an Event sequence to assert against.
    /// </summary>
    /// <typeparam name="T"><see cref="Type" /> of the event that you wish to assert against.</typeparam>
    /// <returns>An <see cref="EventSequenceAssertion{T}" /> scoped to your event type.</returns>
    public EventSequenceAssertion<T> ShouldHavePublicEvent<T>()
        where T : class
        => ShouldHaveEvent<T>(true);
    
    EventSequenceAssertion<T> ShouldHaveEvent<T>(bool isPublic)
        where T : class
    {
        var events = _events.Where(_ => _.Public == isPublic).Select(_ => _.Event).OfType<T>();
        if (!events.Any())
        {
            Throw($"there are no {(isPublic ? "public": "")} {typeof(T)} events.");
        }
        var sequenceValidation = new EventSequenceAssertion<T>(_events.ToList(), isPublic, Throw);
        return sequenceValidation;
    }
    
    /// <summary>
    /// Asserts that the specified event type is not present in the event stream.
    /// </summary>
    /// <typeparam name="T"><see cref="Type" /> of the event that you wish to assert against.</typeparam>
    public void ShouldNotHaveEvent<T>()
        where T : class
        => ShouldNotHaveEvent<T>(false);

    /// <summary>
    /// Asserts that the specified event type is not present in the event stream.
    /// </summary>
    /// <typeparam name="T"><see cref="Type" /> of the event that you wish to assert against.</typeparam>
    public void ShouldNotHavePublicEvent<T>()
        where T : class
        => ShouldNotHaveEvent<T>(true);
    
    void ShouldNotHaveEvent<T>(bool isPublic)
        where T : class
    {
        if (_events.Where(_ => _.Public == isPublic).Select(_ => _.Event).OfType<T>().Any())
        {
            Throw($"there are one or more {(isPublic ? "public": "")} {typeof(T)} events.");
        }
    }

    /// <summary>
    /// Asserts that the event stream does not contain any events.
    /// </summary>
    public void ShouldHaveNoEvents()
        => ShouldHaveNumberOfEvents(0);

    /// <summary>
    /// Asserts that the event stream does not contain any events.
    /// </summary>
    /// <param name="expectedNumberOfEvents">The number of events you wish to assert are present in the stream.</param>
    public void ShouldHaveNumberOfEvents(int expectedNumberOfEvents)
    {
        var numEvents = _events.Length;
        if (numEvents != expectedNumberOfEvents)
        {
            Throw($"expected {expectedNumberOfEvents} events, but {numEvents} events was applied.");
        }
    }

    /// <summary>
    /// Perform a set of assertions on the <typeparamref name="TAggregate"/> instance.
    /// </summary>
    /// <param name="assertions">The set of assertions to perform.</param>
    public void Where(params Action<TAggregate>[] assertions)
    {
        foreach (var assertion in assertions)
        {
            assertion(_aggregate);
        }
    }

}

