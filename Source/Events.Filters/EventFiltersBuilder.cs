// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using Dolittle.SDK.Events.Processing;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Events.Filters
{
    /// <summary>
    /// Represents the builder for building event filters.
    /// </summary>
    public class EventFiltersBuilder
    {
        readonly IList<PrivateEventFilterBuilder> _privateFilterBuilders = new List<PrivateEventFilterBuilder>();
        readonly IList<PublicEventFilterBuilder> _publicFilterBuilders = new List<PublicEventFilterBuilder>();

        /// <summary>
        /// Start building for a private filter.
        /// </summary>
        /// <param name="filterId">The <see cref="FilterId" />.</param>
        /// <param name="callback">Callback for building the event filter.</param>
        /// <returns>Continuation of the builder.</returns>
        public EventFiltersBuilder CreatePrivateFilter(FilterId filterId, Action<PrivateEventFilterBuilder> callback)
        {
            var builder = new PrivateEventFilterBuilder(filterId);
            callback(builder);
            _privateFilterBuilders.Add(builder);
            return this;
        }

        /// <summary>
        /// Start building for a public filter.
        /// </summary>
        /// <param name="filterId">The <see cref="FilterId" />.</param>
        /// <param name="callback">Callback for building the event filter.</param>
        /// <returns>Continuation of the builder.</returns>
        public EventFiltersBuilder CreatePublicFilter(FilterId filterId, Action<PublicEventFilterBuilder> callback)
        {
            var builder = new PublicEventFilterBuilder(filterId);
            callback(builder);
            _publicFilterBuilders.Add(builder);
            return this;
        }

        /// <summary>
        /// Builds and registers all the event filters.
        /// </summary>
        /// <param name="eventProcessors">The <see cref="IEventProcessors" />.</param>
        /// <param name="converter">The <see cref="IEventProcessingConverter" />.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory" />.</param>
        /// <param name="cancellation">The <see cref="CancellationToken" />.</param>
        public void BuildAndRegister(
            IEventProcessors eventProcessors,
            IEventProcessingConverter converter,
            ILoggerFactory loggerFactory,
            CancellationToken cancellation)
        {
            foreach (var filterBuilder in _privateFilterBuilders)
            {
                filterBuilder.BuildAndRegister(eventProcessors, converter, loggerFactory, cancellation);
            }

            foreach (var filterBuilder in _publicFilterBuilders)
            {
                filterBuilder.BuildAndRegister(eventProcessors, converter, loggerFactory, cancellation);
            }
        }
    }
}