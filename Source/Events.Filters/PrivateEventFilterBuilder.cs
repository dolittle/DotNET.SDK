// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Dolittle.SDK.Events.Processing;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Events.Filters
{
    /// <summary>
    /// Represents the builder for building private event filters.
    /// </summary>
    public class PrivateEventFilterBuilder
    {
        IPrivateFilterBuilder _innerBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrivateEventFilterBuilder"/> class.
        /// </summary>
        /// <param name="filterId">The <see cref="FilterId" />.</param>
        public PrivateEventFilterBuilder(FilterId filterId) => FilterId = filterId;

        /// <summary>
        /// Gets the <see cref="FilterId" /> of the filter that this builder builds.
        /// </summary>
        public FilterId FilterId { get; }

        /// <summary>
        /// Gets the <see cref="ScopeId" /> the filter operates on.
        /// </summary>
        public ScopeId ScopeId { get; private set; } = ScopeId.Default;

        /// <summary>
        /// Defines which <see cref="ScopeId" /> the filter operates on.
        /// </summary>
        /// <param name="scopeId">The <see cref="ScopeId" />.</param>
        /// <returns>The builder instance.</returns>
        public PrivateEventFilterBuilder InScope(ScopeId scopeId)
        {
            ScopeId = scopeId;
            return this;
        }

        /// <summary>
        /// Defines the filter to be partitioned.
        /// </summary>
        /// <returns>The partitioned event filter builder continuation.</returns>
        public PartitionedEventFilterBuilder Partitioned()
        {
            var builder = new PartitionedEventFilterBuilder();
            _innerBuilder = builder;
            return builder;
        }

        /// <summary>
        /// Defines the filter to not be partitioned.
        /// </summary>
        /// <returns>The unpartitioned event filter builder continuation.</returns>
        public UnpartitionedEventFilterBuilder Unpartitioned()
        {
            var builder = new UnpartitionedEventFilterBuilder();
            _innerBuilder = builder;
            return builder;
        }

        /// <summary>
        /// Builds and register an instance of a private filter.
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
            if (_innerBuilder == default) throw new FilterDefinitionIncomplete(FilterId, ScopeId, "Call Partitioned() or Handle(...) before building private filter");
            _innerBuilder.BuildAndRegister(FilterId, ScopeId, eventProcessors, converter, loggerFactory, cancellation);
        }
    }
}
