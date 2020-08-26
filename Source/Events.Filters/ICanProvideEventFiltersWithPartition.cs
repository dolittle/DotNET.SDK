// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Events.Filters.Internal;

namespace Dolittle.Events.Filters
{
    /// <summary>
    /// Represents a system that is capable of providing implementations of <see cref="ICanFilterEventsWithPartition"/>.
    /// </summary>
    public interface ICanProvideEventFiltersWithPartition : ICanProvideFilters<ICanFilterEventsWithPartition, IEvent, PartitionedFilterResult>
    {
    }
}