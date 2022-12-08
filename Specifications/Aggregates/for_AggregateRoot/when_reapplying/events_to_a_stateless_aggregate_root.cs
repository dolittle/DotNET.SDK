// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Dolittle.SDK.Events;
using Machine.Specifications;

namespace Dolittle.SDK.Aggregates.for_AggregateRoot.when_reapplying;

public class events_to_a_stateless_aggregate_root : given.committed_events_and_two_aggregate_roots
{

    Because of = () => stateless_aggregate_root.Rehydrate(build_committed_events(event_source_id, stateless_aggregate_root.GetAggregateRootId(), execution_context), CancellationToken.None).GetAwaiter().GetResult();

    It should_be_at_version_three = () => stateless_aggregate_root.Version.ShouldEqual<AggregateRootVersion>(3);
    It should_have_no_uncommitted_events = () => stateless_aggregate_root.AppliedEvents.ShouldBeEmpty();
}