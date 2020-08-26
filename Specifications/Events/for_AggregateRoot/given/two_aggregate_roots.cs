﻿// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Events;
using Machine.Specifications;
using given = Dolittle.Events.given;

namespace Dolittle.Domain.for_AggregateRoot.given
{
    public abstract class two_aggregate_roots : given::Events
    {
        public static EventSourceId event_source_id = Guid.Parse("244762a3-38bc-422c-8197-99c0ca33b5d6");

        public static StatelessAggregateRoot stateless_aggregate_root;
        public static StatefulAggregateRoot statefull_aggregate_root;

        Establish context = () =>
        {
            stateless_aggregate_root = new StatelessAggregateRoot(event_source_id);
            statefull_aggregate_root = new StatefulAggregateRoot(event_source_id);
        };
    }
}
