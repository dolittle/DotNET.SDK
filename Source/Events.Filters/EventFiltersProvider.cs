// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Dolittle.Events.Filters.Internal;
using Dolittle.Logging;
using Dolittle.Types;

namespace Dolittle.Events.Filters
{
    /// <summary>
    /// Represents an implementation of <see cref="ICanProvideEventFilters"/> and <see cref="ICanProvideEventFilters"/> that provides event filters.
    /// </summary>
    public class EventFiltersProvider : ICanProvideEventFilters, ICanProvideEventFiltersWithPartition
    {
        readonly IEnumerable<ICanFilterEvents> _unpartitionedFilters;
        readonly IEnumerable<ICanFilterEventsWithPartition> _partitionedFilters;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventFiltersProvider"/> class.
        /// </summary>
        /// <param name="unpartitionedFilters"><see cref="IInstancesOf{T}"/> of type <see cref="ICanFilterEvents"/>.</param>
        /// <param name="partitionedFilters"><see cref="IInstancesOf{T}"/> of type <see cref="ICanFilterEventsWithPartition"/>.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public EventFiltersProvider(IInstancesOf<ICanFilterEvents> unpartitionedFilters, IInstancesOf<ICanFilterEventsWithPartition> partitionedFilters, ILogger logger)
        {
            _unpartitionedFilters = unpartitionedFilters;
            _partitionedFilters = partitionedFilters;
            _logger = logger;
        }

        /// <inheritdoc/>
        IEnumerable<ICanFilterEvents> ICanProvideFilters<ICanFilterEvents, IEvent, FilterResult>.Provide()
        {
            _logger.Debug("Providing {FilterCount} unpartitioned event filters", _unpartitionedFilters.Count());
            return _unpartitionedFilters;
        }

        /// <inheritdoc/>
        IEnumerable<ICanFilterEventsWithPartition> ICanProvideFilters<ICanFilterEventsWithPartition, IEvent, PartitionedFilterResult>.Provide()
        {
            _logger.Debug("Providing {FilterCount} partitioned event filters", _partitionedFilters.Count());
            return _partitionedFilters;
        }
    }
}