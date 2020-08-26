// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Events.Filters.Internal;

namespace Dolittle.Events.Filters
{
    /// <summary>
    /// Defines a system that can filter instances of <see cref="IEvent"/> to a partitioned stream.
    /// </summary>
    public interface ICanFilterEventsWithPartition : ICanFilter<IEvent, PartitionedFilterResult>
    {
    }
}