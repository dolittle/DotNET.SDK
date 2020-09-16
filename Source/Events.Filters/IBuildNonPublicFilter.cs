// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Dolittle.SDK.Events.Processing;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Events.Filters
{
    /// <summary>
    /// Defines a builder that can build a non-public filter.
    /// </summary>
    public interface IBuildNonPublicFilter
    {
        /// <summary>
        /// Builds and register an instance of a private filter.
        /// </summary>
        /// <param name="filterId">The <see cref="FilterId" />.</param>
        /// <param name="scopeId">The <see cref="ScopeId" />.</param>
        /// <param name="eventProcessors">The <see cref="IEventProcessors" />.</param>
        /// <param name="eventProcessingRequestConverter">The <see cref="IEventProcessingRequestConverter" />.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory" />.</param>
        /// <param name="cancellation">The <see cref="CancellationToken" />.</param>
        void BuildAndRegister(
            FilterId filterId,
            ScopeId scopeId,
            IEventProcessors eventProcessors,
            IEventProcessingRequestConverter eventProcessingRequestConverter,
            ILoggerFactory loggerFactory,
            CancellationToken cancellation);
    }
}
