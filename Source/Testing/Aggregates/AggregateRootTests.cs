// Copyright (c) Dolittle. All rights reserved.


using System;
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Testing.Aggregates;

public abstract class AggregateRootTests<T> where T : AggregateRoot
{
    readonly EventSourceId _eventSourceId;
    readonly AggregateOfMock<T> _aggregateOf;
    public AggregateRootAssertion AssertThat { get; private set; } = null!;

    protected IAggregateRootOperations<T> Aggregate { get; }

    protected AggregateRootTests(Func<EventSourceId, T> getAggregate, EventSourceId eventSourceId)
    {
        _eventSourceId = eventSourceId;
        _aggregateOf = new AggregateOfMock<T>(getAggregate);
        Aggregate = _aggregateOf.Get(eventSourceId);
    }

    protected void WithAggregateInState(Action<T> action)
    {
        Aggregate.Perform(action).GetAwaiter().GetResult();
    }

    protected AggregateRootAssertion WhenPerforming(Action<T> action)
    {
        // last perform is the one that is asserted on
        Aggregate.Perform(action).GetAwaiter().GetResult();
        AssertThat = _aggregateOf.AfterLastOperationOn(_eventSourceId);
        return AssertThat;
    }
}