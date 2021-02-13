// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events;
using Machine.Specifications;

namespace Dolittle.SDK.Aggregates.for_AggregateRoot
{
    public class when_creating : given.two_aggregate_roots
    {
        static given.StatelessAggregateRoot aggregate_root;

        Because of = () => aggregate_root = new given.StatelessAggregateRoot(event_source_id);

        It should_be_the_correct_event_source_id = () => aggregate_root.EventSourceId.ShouldEqual(event_source_id);
        It should_have_the_initial_version = () => aggregate_root.Version.ShouldEqual(AggregateRootVersion.Initial);
        It should_have_no_uncommitted_events = () => aggregate_root.AppliedEvents.ShouldBeEmpty();
    }
}
