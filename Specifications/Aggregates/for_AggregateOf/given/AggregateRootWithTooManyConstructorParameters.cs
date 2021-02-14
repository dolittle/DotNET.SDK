// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events;

namespace Dolittle.SDK.Aggregates.for_AggregateOf.given
{
    [AggregateRoot("daaef579-b711-4339-bd09-e04f53cc01b5")]
    public class AggregateRootWithTooManyConstructorParameters : AggregateRoot
    {
        public AggregateRootWithTooManyConstructorParameters(EventSourceId event_source, EventSourceId some_other)
            : base(event_source)
        {
        }
    }
}