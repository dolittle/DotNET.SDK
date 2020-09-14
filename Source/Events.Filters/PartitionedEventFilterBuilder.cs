// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events.Processing;
using Dolittle.SDK.Services;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Events.Filters
{
    /// <summary>
    /// Represents the builder for building partitioned event filters.
    /// </summary>
    public class PartitionedEventFilterBuilder : IBuildNonPublicFilter
    {
        PartitionedFilterEventCallback _callback;

        /// <summary>
        /// Defines a callback for the filter.
        /// </summary>
        /// <param name="callback">The callback that will be called for each event.</param>
        public void Handle(PartitionedFilterEventCallback callback)
            => _callback = callback;

        /// <inheritdoc/>
        public IFilterProcessor Build(
            FilterId filterId,
            ScopeId scopeId,
            ICreateReverseCallClients reverseCallClientCreator,
            IEventProcessingRequestConverter eventProcessingRequestConverter,
            ILoggerFactory loggerFactory)
        {
            ThrowIfCallbackIsMissing(filterId, scopeId);
            return new Internal.PartitionedEventFilterProcessor(
                filterId,
                scopeId,
                _callback,
                reverseCallClientCreator,
                eventProcessingRequestConverter,
                loggerFactory.CreateLogger<Internal.EventFilterProcessor>());
        }

        void ThrowIfCallbackIsMissing(FilterId filterId, ScopeId scopeId)
        {
            if (_callback == default) throw new MissingFilterCallback(filterId, scopeId);
        }
    }
}
