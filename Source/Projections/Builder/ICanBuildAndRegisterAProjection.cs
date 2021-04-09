// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Processing;
using Dolittle.SDK.Projections.Store.Converters;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Projections.Builder
{
    /// <summary>
    /// Defines a system that can build and register a projection.
    /// </summary>
    public interface ICanBuildAndRegisterAProjection
    {
        /// <summary>
        /// Builds and registers the projection.
        /// </summary>
        /// <param name="eventProcessors">The <see cref="IEventProcessors" />.</param>
        /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
        /// <param name="processingConverter">The <see cref="IEventProcessingConverter" />.</param>
        /// <param name="projectionConverter">The <see cref="IConvertProjectionsToSDK" />.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory" />.</param>
        /// <param name="cancellation">The <see cref="CancellationToken" />.</param>
        void BuildAndRegister(
            IEventProcessors eventProcessors,
            IEventTypes eventTypes,
            IEventProcessingConverter processingConverter,
            IConvertProjectionsToSDK projectionConverter,
            ILoggerFactory loggerFactory,
            CancellationToken cancellation);
    }
}
