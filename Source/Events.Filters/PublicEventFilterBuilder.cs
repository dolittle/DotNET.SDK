// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events.Processing;
using Dolittle.SDK.Services;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Events.Filters
{
    /// <summary>
    /// Represents the builder for building public event filters.
    /// </summary>
    public class PublicEventFilterBuilder
    {
        PartitionedFilterEventCallback _callback;

        /// <summary>
        /// Initializes a new instance of the <see cref="PublicEventFilterBuilder"/> class.
        /// </summary>
        /// <param name="filterId">The <see cref="FilterId" />.</param>
        public PublicEventFilterBuilder(FilterId filterId) => FilterId = filterId;

        /// <summary>
        /// Gets the <see cref="FilterId" /> of the filter that this builder builds.
        /// </summary>
        public FilterId FilterId { get; }

        /// <summary>
        /// Defines a callback for the filter.
        /// </summary>
        /// <param name="callback">The callback that will be called for each event.</param>
        public void Handle(PartitionedFilterEventCallback callback)
            => _callback = callback;

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
            ThrowIfCallbackIsMissing();
            return new Internal.PublicEventFilterProcessor(
                FilterId,
                _callback,
                reverseCallClientCreator,
                eventProcessingRequestConverter,
                loggerFactory.CreateLogger<Internal.EventFilterProcessor>());
        }

        void ThrowIfCallbackIsMissing()
        {
            if (_callback == default) throw new MissingFilterCallback(FilterId, ScopeId.Default);
        }
    }
}