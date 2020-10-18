// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Store;
using Machine.Specifications;

namespace Dolittle.SDK.Aggregates.for_AggregateRoot.when_applying
{
    public class null_event : given.two_aggregate_roots
    {
        static Exception exception;

        Because of = () => exception = Catch.Exception(() => stateless_aggregate_root.Apply(null));

        It should_fail_because_event_content_is_null = () => exception.ShouldBeOfExactType<EventContentCannotBeNull>();
        It should_not_increment_the_version = () => stateless_aggregate_root.Version.ShouldEqual(AggregateRootVersion.Initial);
        It should_have_no_uncommitted_events = () => stateless_aggregate_root.UncommittedEvents.ShouldBeEmpty();
    }
}
