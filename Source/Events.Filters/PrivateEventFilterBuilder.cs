// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events.Processing;
using Dolittle.SDK.Services;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Events.Filters
{
    /// <summary>
    /// Represents the builder for building private event filters.
    /// </summary>
    public class PrivateEventFilterBuilder
    {
        IBuildNonPublicFilter _innerBuilder;

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
            _innerBuilder = new PartitionedEventFilterBuilder();
            return _innerBuilder as PartitionedEventFilterBuilder;
        }

        /// <summary>
        /// Defines a callback for the filter.
        /// </summary>
        /// <param name="callback">The callback that will be called for each event.</param>
        public void Handle(FilterEventCallback callback)
        {
            _innerBuilder = new UnpartitionedEventFilterBuilder();
            (_innerBuilder as UnpartitionedEventFilterBuilder).Handle(callback);
        }

        /// <summary>
        /// Build an instance of <see cref="IFilterProcessor" />.
        /// </summary>
        /// <param name="reverseCallClientCreator">The <see cref="ICreateReverseCallClients" />.</param>
        /// <param name="eventProcessingRequestConverter">The <see cref="IEventProcessingRequestConverter" />.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory" />.</param>
        /// <returns>The <see cref="IFilterProcessor" /> instance.</returns>
        public IFilterProcessor Build(
            ICreateReverseCallClients reverseCallClientCreator,
            IEventProcessingRequestConverter eventProcessingRequestConverter,
            ILoggerFactory loggerFactory)
        {
            if (_innerBuilder == default) throw new FilterDefinitionIncomplete(FilterId, ScopeId, "Call Partitioned() or Handle(...)");
            return _innerBuilder.Build(FilterId, ScopeId, reverseCallClientCreator, eventProcessingRequestConverter, loggerFactory);
        }
    }
}
