// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Dolittle.SDK.Events.Filters.Internal;
using Dolittle.SDK.Events.Processing;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Events.Filters
{
    /// <summary>
    /// Represents the builder for building public event filters.
    /// </summary>
    public class PublicEventFilterBuilder
    {
        static readonly PublicEventFilterProtocol _protocol = new PublicEventFilterProtocol();
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
        /// Builds and register an instance of <see cref="PublicEventFilterProcessor" />.
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
            ThrowIfCallbackIsMissing();
            var filter = new PublicEventFilterProcessor(FilterId, _callback, converter, loggerFactory);
            eventProcessors.Register(filter, _protocol, cancellation);
        }

        void ThrowIfCallbackIsMissing()
        {
            if (_callback == default) throw new MissingFilterCallback(FilterId, ScopeId.Default);
        }
    }
}
