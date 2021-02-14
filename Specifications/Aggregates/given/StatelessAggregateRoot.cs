// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events;

namespace Dolittle.SDK.Aggregates.given
{
    [AggregateRoot("b4e48d89-2a2a-4eaa-a071-2b688f8bf8fb")]
    public class StatelessAggregateRoot : AggregateRoot
    {
        public StatelessAggregateRoot(EventSourceId eventSource)
            : base(eventSource)
        {
        }
    }
}