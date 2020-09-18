// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Dolittle.SDK.Events.Filters.Internal;
using Dolittle.SDK.Events.Processing;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Events.Filters
{
    /// <summary>
    /// Represents the builder for building partitioned event filters.
    /// </summary>
    public class PartitionedEventFilterBuilder : IBuildNonPublicFilter
    {
        static readonly PartitionedEventFilterProtocol _protocol = new PartitionedEventFilterProtocol();

        PartitionedFilterEventCallback _callback;

        /// <summary>
        /// Defines a callback for the filter.
        /// </summary>
        /// <param name="callback">The callback that will be called for each event.</param>
        public void Handle(PartitionedFilterEventCallback callback)
            => _callback = callback;

        /// <inheritdoc/>
        public void BuildAndRegister(
            FilterId filterId,
            ScopeId scopeId,
            IEventProcessors eventProcessors,
            IEventProcessingConverter converter,
            ILoggerFactory loggerFactory,
            CancellationToken cancellation)
        {
            ThrowIfCallbackIsMissing(filterId, scopeId);
            var filter = new PartitionedEventFilterProcessor(filterId, scopeId, _callback, converter, loggerFactory);
            eventProcessors.Register(filter, _protocol, cancellation);
        }

        void ThrowIfCallbackIsMissing(FilterId filterId, ScopeId scopeId)
        {
            if (_callback == default) throw new MissingFilterCallback(filterId, scopeId);
        }
    }
}
