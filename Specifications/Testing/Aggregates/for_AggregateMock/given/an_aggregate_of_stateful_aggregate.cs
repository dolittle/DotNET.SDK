// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.SDK.Testing.Aggregates.for_AggregateMock.given;

class an_aggregate_of_stateful_aggregate
{
    protected static AggregateOfMock<StatefulAggregateRoot> aggregate_of;
    
    Establish context = () =>
    {
        aggregate_of = AggregateOfMock<StatefulAggregateRoot>.Create();
    };
}