// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events;
using Machine.Specifications;
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Local

namespace Dolittle.SDK.Testing.Aggregates.for_AggregateMock.when_performing_operations.on_default_constructor_aggregate;

class and_operation_does_not_fail : given.an_aggregate_with_default_constructor
{
    static EventSourceId event_source;

    Establish context = () =>
    {
        event_source = "event source";
    };
    
    Because of = () => aggregate_of.Get(event_source).Perform(_ => _.EventCausingNoStateChange()).GetAwaiter().GetResult();

    It should_after_last_operation_have_the_correct_event = () => aggregate_of.AfterLastOperationOn(event_source)
        .ShouldHaveEvent<EventCausingNoStateChange>()
        .AndThat(_ => _.EventSourceId.ShouldEqual(event_source.Value));
}