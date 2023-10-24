// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Testing.Aggregates.Events;

namespace Dolittle.SDK.Testing.Aggregates;

public static class AggregateRootExtensions
{
    /// <summary>
    /// Gets the <see cref="AggregateRootAssertion"/> instance for the aggregate.
    /// </summary>
    /// <param name="aggregate">The aggregate.</param>
    /// <returns>The assertion.</returns>
    public static AggregateRootAssertion AssertThat(this AggregateRoot aggregate) => aggregate;

    /// <summary>
    /// Starts the Fluent Interface by establishing an Event sequence to assert against.
    /// </summary>
    /// <param name="aggregateRoot">The <see cref="AggregateRoot" /> containing the events to assert against.</param>
    /// <typeparam name="T"><see cref="Type" /> of the event that you wish to assert against.</typeparam>
    /// <returns>An <see cref="EventSequenceAssertion{T}" /> scoped to your event type.</returns>
    public static EventSequenceAssertion<T> ShouldHaveEvent<T>(this AggregateRoot aggregateRoot)
        where T : class
        => aggregateRoot.AssertThat().ShouldHaveEvent<T>();

    /// <summary>
    /// Starts the Fluent Interface by establishing an Event sequence to assert against.
    /// </summary>
    /// <param name="aggregateRoot">The <see cref="AggregateRoot" /> containing the events to assert against.</param>
    /// <typeparam name="T"><see cref="Type" /> of the event that you wish to assert against.</typeparam>
    /// <returns>An <see cref="EventSequenceAssertion{T}" /> scoped to your event type.</returns>
    public static EventSequenceAssertion<T> ShouldHavePublicEvent<T>(this AggregateRoot aggregateRoot)
        where T : class
        => aggregateRoot.AssertThat().ShouldHavePublicEvent<T>();


    /// <summary>
    /// Asserts that the specified event type is not present in the event stream.
    /// </summary>
    /// <param name="aggregateRoot">The <see cref="AggregateRoot" /> containing the events to assert against.</param>
    /// <typeparam name="T"><see cref="Type" /> of the event that you wish to assert against.</typeparam>
    public static void ShouldNotHaveEvent<T>(this AggregateRoot aggregateRoot)
        where T : class
        => aggregateRoot.AssertThat().ShouldNotHaveEvent<T>();

    /// <summary>
    /// Asserts that the specified event type is not present in the event stream.
    /// </summary>
    /// <param name="aggregateRoot">The <see cref="AggregateRoot" /> containing the events to assert against.</param>
    /// <typeparam name="T"><see cref="Type" /> of the event that you wish to assert against.</typeparam>
    public static void ShouldNotHavePublicEvent<T>(this AggregateRoot aggregateRoot)
        where T : class
        => aggregateRoot.AssertThat().ShouldNotHavePublicEvent<T>();

    /// <summary>
    /// Asserts that the event stream does not contain any events.
    /// </summary>
    /// <param name="aggregateRoot">The <see cref="AggregateRoot" /> containing the events to assert against.</param>
    public static void ShouldHaveNoEvents(this AggregateRoot aggregateRoot)
        => aggregateRoot.AssertThat().ShouldHaveNoEvents();

    /// <summary>
    /// Asserts that the event stream does not contain any events.
    /// </summary>
    /// <param name="aggregateRoot">The <see cref="AggregateRoot" /> containing the events to assert against.</param>
    /// <param name="expectedNumberOfEvents">The number of events you wish to assert are present in the stream.</param>
    public static void ShouldHaveNumberOfEvents(this AggregateRoot aggregateRoot, int expectedNumberOfEvents)
        => aggregateRoot.AssertThat().ShouldHaveNumberOfEvents(expectedNumberOfEvents);
}
