// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Dolittle.Logging;
using Dolittle.Types;

namespace Dolittle.Events.Filters.EventHorizon
{
    /// <summary>
    /// Represents an implementation of <see cref="ICanProvidePublicEventFilters" /> that provides filters that filter public events.
    /// </summary>
    public class PublicEventsFilterProvider : ICanProvidePublicEventFilters
    {
        readonly IEnumerable<ICanFilterPublicEvents> _filters;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PublicEventsFilterProvider"/> class.
        /// </summary>
        /// <param name="filters">The <see cref="IInstancesOf{T}" /> for <see cref="ICanFilterPublicEvents" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public PublicEventsFilterProvider(IInstancesOf<ICanFilterPublicEvents> filters, ILogger logger)
        {
            _filters = filters;
            _logger = logger;
        }

        /// <inheritdoc/>
        public IEnumerable<ICanFilterPublicEvents> Provide()
        {
            _logger.Debug("Providing {FilterCount} public event filters", _filters.Count());
            return _filters;
        }
    }
}