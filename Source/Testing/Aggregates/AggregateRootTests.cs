﻿// Copyright (c) Dolittle. All rights reserved.


using System;
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.SDK.Testing.Aggregates;

/// <summary>
/// Base class for testing <see cref="AggregateRoot"/> instances.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class AggregateRootTests<T> where T : AggregateRoot
{
    readonly EventSourceId _eventSourceId;
    readonly AggregateOfMock<T> _aggregateOf;

    /// <summary>
    /// Gets the <see cref="AggregateRootAssertion"/> that allows for asserting on the aggregate root.
    /// </summary>
    public AggregateRootAssertion AssertThat { get; private set; } = null!;

    /// <summary>
    /// Gets the <see cref="IAggregateRootOperations{T}"/> for the aggregate root.
    /// </summary>
    protected IAggregateRootOperations<T> Aggregate { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateRootTests{T}"/> class.
    /// </summary>
    /// <param name="eventSourceId">The ID of the aggregate instance under test</param>
    /// <param name="configureServices">If any services are required, they can be configured in this optional callback</param>
    protected AggregateRootTests(EventSourceId eventSourceId, Action<IServiceCollection>? configureServices = default)
    {
        _eventSourceId = eventSourceId;
        _aggregateOf = AggregateOfMock<T>.Create(configureServices);
        Aggregate = _aggregateOf.Get(eventSourceId);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateRootTests{T}"/> class.
    /// </summary>
    /// <param name="getAggregate">Callback to produce the aggregate instance from the ID</param>
    /// <param name="eventSourceId">The ID of the aggregate instance under test</param>
    protected AggregateRootTests(Func<EventSourceId, T> getAggregate, EventSourceId eventSourceId)
    {
        _eventSourceId = eventSourceId;
        _aggregateOf = new AggregateOfMock<T>(id =>
        {
            var aggregate = getAggregate(id);
            try
            {
                aggregate.EventSourceId = id;
            }
            catch
            {
                // ignored
            }

            return aggregate;
        });
        Aggregate = _aggregateOf.Get(eventSourceId);
    }

    /// <summary>
    /// Use this method to set up the aggregate in a specific state before performing a test action.
    /// Events output by this will not be included in the assertion, but they will be applied to the aggregate.
    /// </summary>
    /// <param name="action">The callback on the aggregate</param>
    protected void WithAggregateInState(Action<T> action)
    {
        Aggregate.Perform(action).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Perform an action on the aggregate and make the output events available to assert on the result.
    /// </summary>
    /// <param name="action">The callback on the aggregate</param>
    /// <returns>The <see cref="AggregateRootAssertion"/> to assert on</returns>
    protected AggregateRootAssertion WhenPerforming(Action<T> action)
    {
        try
        {
            // last perform is the one that is asserted on
            Aggregate.Perform(action).GetAwaiter().GetResult();
            AssertThat = _aggregateOf.AfterLastOperationOn(_eventSourceId);
            return AssertThat;
        }
        catch (AggregateRootOperationFailed e)
        {
            // Allow callers to assert on the domain exception instead of the wrapper.
            throw e.InnerException ?? e;
        }
    }

    /// <summary>
    /// Verify that an exception is thrown when performing an action on the aggregate.
    /// </summary>
    /// <param name="action">Aggregate callback</param>
    /// <exception cref="DolittleAssertionFailed">Thrown when expectation is not met.</exception>
    protected Exception VerifyThrows(Action<T> action) => VerifyThrowsExactly<Exception>(action);

    /// <summary>
    /// Verify that an exception is thrown when performing an action
    /// </summary>
    /// <param name="action">Aggregate callback</param>
    /// <exception cref="DolittleAssertionFailed">Thrown when expectation is not met.</exception>
    protected Exception VerifyThrows(Action action) => VerifyThrowsExactly<Exception>(action);

    /// <summary>
    /// Verify that an exception of the specific type is thrown when performing an action on the aggregate.
    /// </summary>
    /// <param name="action">Aggregate callback</param>
    /// <typeparam name="TException">The expected Exception type to be thrown</typeparam>
    /// <exception cref="DolittleAssertionFailed">Thrown when expectation is not met.</exception>
    protected TException VerifyThrowsExactly<TException>(Action<T> action) where TException : Exception =>
        VerifyThrowsExactly<TException>(() => WhenPerforming(action));

    /// <summary>
    /// Verify that an exception of the specific type is thrown when performing an action
    /// </summary>
    /// <param name="action">Aggregate callback</param>
    /// <typeparam name="TException">The expected Exception type to be thrown</typeparam>
    /// <exception cref="DolittleAssertionFailed">Thrown when expectation is not met.</exception>
    protected TException VerifyThrowsExactly<TException>(Action action) where TException : Exception
    {
        try
        {
            action();
            throw new DolittleAssertionFailed(
                $"Expected exception of type {typeof(TException).Name} but no exception was thrown");
        }
        catch (Exception e)
        {
            if (e is TException exception)
            {
                // Expectation met
                return exception;
            }

            throw new DolittleAssertionFailed(
                $"Expected exception of type {typeof(TException).Name} but got {e.GetType().Name}");
        }
    }
}
