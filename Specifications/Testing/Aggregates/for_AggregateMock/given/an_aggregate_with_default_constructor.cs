// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.SDK.Testing.Aggregates.for_AggregateMock.given;

class an_aggregate_with_default_constructor
{
    protected static AggregateOfMock<AggregateRootWithDefaultConstructor> aggregate_of;
    
    Establish context = () =>
    {
        aggregate_of = AggregateOfMock<AggregateRootWithDefaultConstructor>.Create();
    };
}