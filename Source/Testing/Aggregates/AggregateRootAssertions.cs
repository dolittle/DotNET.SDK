// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Aggregates;
using System;
using System.Linq;
using Dolittle.SDK.Testing.Aggregates.Events;
using Dolittle.SDK.Testing.Assertion;

namespace Dolittle.SDK.Testing.Aggregates;

/// <summary>
/// Start of the fluent interface for asserting against applied events of an <see cref="AggregateRoot" />.
/// </summary>
public static class AggregateRootAssertions
{
    static void Throw(this AggregateRoot aggregateRoot, string reason)
        => AggregateAssertionFailed.Throw(aggregateRoot, reason);
    
        /// <summary>
    /// Starts the Fluent Interface by establishing an Event sequence to assert against.
    /// </summary>
    /// <param name="aggregateRoot">The <see cref="AggregateRoot" /> containing the events to assert against.</param>
    /// <typeparam name="T"><see cref="Type" /> of the event that you wish to assert against.</typeparam>
    /// <returns>An <see cref="EventSequenceAssertion{T}" /> scoped to your event type.</returns>
    public static EventSequenceAssertion<T> ShouldHaveEvent<T>(this AggregateRoot aggregateRoot)
        where T : class
        => ShouldHaveEvent<T>(aggregateRoot, false);

    /// <summary>
    /// Starts the Fluent Interface by establishing an Event sequence to assert against.
    /// </summary>
    /// <param name="aggregateRoot">The <see cref="AggregateRoot" /> containing the events to assert against.</param>
    /// <typeparam name="T"><see cref="Type" /> of the event that you wish to assert against.</typeparam>
    /// <returns>An <see cref="EventSequenceAssertion{T}" /> scoped to your event type.</returns>
    public static EventSequenceAssertion<T> ShouldHavePublicEvent<T>(this AggregateRoot aggregateRoot)
        where T : class
        => ShouldHaveEvent<T>(aggregateRoot, true);
    
    static EventSequenceAssertion<T> ShouldHaveEvent<T>(this AggregateRoot aggregateRoot, bool isPublic)
        where T : class
    {
        var events = aggregateRoot.AppliedEvents.Where(_ => _.Public == isPublic).Select(_ => _.Event).OfType<T>();
        if (!events.Any())
        {
            aggregateRoot.Throw($"there are no {(isPublic ? "public": "")} {typeof(T)} events.");
        }
        var sequenceValidation = new EventSequenceAssertion<T>(aggregateRoot.AppliedEvents.ToList(), isPublic, aggregateRoot.Throw);
        return sequenceValidation;
    }
    
    /// <summary>
    /// Asserts that the specified event type is not present in the event stream.
    /// </summary>
    /// <param name="aggregateRoot">The <see cref="AggregateRoot" /> containing the events to assert against.</param>
    /// <typeparam name="T"><see cref="Type" /> of the event that you wish to assert against.</typeparam>
    public static void ShouldNotHaveEvent<T>(this AggregateRoot aggregateRoot)
        where T : class
        => ShouldNotHaveEvent<T>(aggregateRoot, false);

    /// <summary>
    /// Asserts that the specified event type is not present in the event stream.
    /// </summary>
    /// <param name="aggregateRoot">The <see cref="AggregateRoot" /> containing the events to assert against.</param>
    /// <typeparam name="T"><see cref="Type" /> of the event that you wish to assert against.</typeparam>
    public static void ShouldNotHavePublicEvent<T>(this AggregateRoot aggregateRoot)
        where T : class
        => ShouldNotHaveEvent<T>(aggregateRoot, true);
    
    static void ShouldNotHaveEvent<T>(this AggregateRoot aggregateRoot, bool isPublic)
        where T : class
    {
        if (aggregateRoot.AppliedEvents.Where(_ => _.Public == isPublic).Select(_ => _.Event).OfType<T>().Any())
        {
            aggregateRoot.Throw($"there are one or more {(isPublic ? "public": "")} {typeof(T)} events.");
        }
    }

    /// <summary>
    /// Asserts that the event stream does not contain any events.
    /// </summary>
    /// <param name="eventSource">The <see cref="AggregateRoot" /> containing the events to assert against.</param>
    public static void ShouldHaveNoEvents(this AggregateRoot eventSource)
    {
        eventSource.ShouldHaveNumberOfEvents(0);
    }

    /// <summary>
    /// Asserts that the event stream does not contain any events.
    /// </summary>
    /// <param name="aggregateRoot">The <see cref="AggregateRoot" /> containing the events to assert against.</param>
    /// <param name="expectedNumberOfEvents">The number of events you wish to assert are present in the stream.</param>
    public static void ShouldHaveNumberOfEvents(this AggregateRoot aggregateRoot, int expectedNumberOfEvents)
    {
        var numEvents = aggregateRoot.AppliedEvents.Count();
        if (numEvents != expectedNumberOfEvents)
        {
            aggregateRoot.Throw($"expected {expectedNumberOfEvents} events, but {numEvents} events was applied.");
        }
    }
}
