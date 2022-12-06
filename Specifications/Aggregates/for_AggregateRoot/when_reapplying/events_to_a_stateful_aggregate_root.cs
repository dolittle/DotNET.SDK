// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Dolittle.SDK.Events;
using Machine.Specifications;

namespace Dolittle.SDK.Aggregates.for_AggregateRoot.when_reapplying;

public class events_to_a_stateful_aggregate_root : given.committed_events_and_two_aggregate_roots
{
    Because of = () => statefull_aggregate_root.Rehydrate(build_committed_events(event_source_id, statefull_aggregate_root.GetAggregateRootId(), execution_context), CancellationToken.None).GetAwaiter().GetResult();

    It should_be_at_version_three = () => statefull_aggregate_root.Version.ShouldEqual<AggregateRootVersion>(3);
    It should_handle_simple_event_two_times = () => statefull_aggregate_root.AnEventOnCalled.ShouldEqual(2);
    It should_handle_another_event_one_time = () => statefull_aggregate_root.AnotherEventOnCalled.ShouldEqual(1);
    It should_have_no_uncommitted_events = () => statefull_aggregate_root.AppliedEvents.ShouldBeEmpty();
}