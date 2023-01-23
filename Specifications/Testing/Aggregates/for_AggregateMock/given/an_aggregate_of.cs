// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;
using Machine.Specifications;
using Moq;
using It = Moq.It;

namespace Dolittle.SDK.Testing.Aggregates.for_AggregateMock.given;

class an_aggregate_of<TAggregate>
    where TAggregate : AggregateRoot
{
    protected static Mock<Func<EventSourceId, TAggregate>> aggregate_factory;
    protected static AggregateOfMock<TAggregate> aggregate_of => _aggregate_of ??= new(aggregate_factory.Object) ;

    static AggregateOfMock<TAggregate> _aggregate_of;

    Establish context = () =>
    {
        _aggregate_of = null;
        aggregate_factory = null;
    };

    protected static void use_aggregate_factory(Func<EventSourceId, TAggregate> factory)
    {
        aggregate_factory = new Mock<Func<EventSourceId, TAggregate>>();
        aggregate_factory.Setup(_ => _.Invoke(It.IsAny<EventSourceId>())).Returns(factory);
    }
}