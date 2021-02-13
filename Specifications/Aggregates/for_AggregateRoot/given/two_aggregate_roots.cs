// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events;
using Machine.Specifications;

namespace Dolittle.SDK.Aggregates.for_AggregateRoot.given
{
    public abstract class two_aggregate_roots : events_and_event_types
    {
        protected static AggregateRootId stateful_aggregate_root_id = "bab0924f-0dfc-4d79-8bab-ace680d6648c";
        protected static AggregateRootId stateless_aggregate_root_id = "b4e48d89-2a2a-4eaa-a071-2b688f8bf8fb";
        protected static EventSourceId event_source_id;

        protected static StatelessAggregateRoot stateless_aggregate_root;
        protected static StatefulAggregateRoot statefull_aggregate_root;

        Establish context = () =>
        {
            event_source_id = "244762a3-38bc-422c-8197-99c0ca33b5d6";
            stateless_aggregate_root = new StatelessAggregateRoot(event_source_id);
            statefull_aggregate_root = new StatefulAggregateRoot(event_source_id);
        };
    }
}
