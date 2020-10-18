// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Store;
using Machine.Specifications;

namespace Dolittle.SDK.Aggregates.for_AggregateRoot.when_reapplying
{
    public class events_to_a_stateless_aggregate_root : given.committed_events_and_two_aggregate_roots
    {
        static CommittedAggregateEvents events;

        Establish context = () => events = build_committed_events(event_source_id, stateless_aggregate_root.GetAggregateRootId(), execution_context);

        Because of = () => stateless_aggregate_root.ReApply(events);

        It should_be_at_version_three = () => stateless_aggregate_root.Version.ShouldEqual<AggregateRootVersion>(3);
        It should_have_no_uncommitted_events = () => stateless_aggregate_root.UncommittedEvents.ShouldBeEmpty();
    }
}
