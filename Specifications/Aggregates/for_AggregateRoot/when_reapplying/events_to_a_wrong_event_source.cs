// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Store;
using Machine.Specifications;

namespace Dolittle.SDK.Aggregates.for_AggregateRoot.when_reapplying;

public class events_to_a_wrong_event_source : given.committed_events_and_two_aggregate_roots
{
    static CommittedAggregateEvents events;
    static Exception exception;

    Establish context = () => events = build_committed_events("aa81d146-0c45-4121-bce0-fcc547257ccd", statefull_aggregate_root.GetAggregateRootId(), execution_context);

    Because of = () => exception = Catch.Exception(() => statefull_aggregate_root.ReApply(events));

    It should_throw_an_exception = () => exception.ShouldBeOfExactType<EventWasAppliedToOtherEventSource>();
    It should_be_at_version_three = () => statefull_aggregate_root.Version.ShouldEqual(AggregateRootVersion.Initial);
    It should_have_no_uncommitted_events = () => stateless_aggregate_root.AppliedEvents.ShouldBeEmpty();
}