// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events.Processing;
using Dolittle.SDK.Services;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Events.Filters
{
    /// <summary>
    /// Defines a builder that can build a non-public filter.
    /// </summary>
    public interface IBuildNonPublicFilter
    {
        /// <summary>
        /// Build an instance of <see cref="IFilterProcessor" />.
        /// </summary>
        /// <param name="filterId">Unique identifier for the filter.</param>
        /// <param name="scopeId">The <see cref="ScopeId" />.</param>
        /// <param name="reverseCallClientCreator">The <see cref="ICreateReverseCallClients" />.</param>
        /// <param name="eventProcessingRequestConverter">The <see cref="IEventProcessingRequestConverter" />.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory" />.</param>
        /// <returns>The <see cref="IFilterProcessor" /> instance.</returns>
        IFilterProcessor Build(
            FilterId filterId,
            ScopeId scopeId,
            ICreateReverseCallClients reverseCallClientCreator,
            IEventProcessingRequestConverter eventProcessingRequestConverter,
            ILoggerFactory loggerFactory);
    }
}
