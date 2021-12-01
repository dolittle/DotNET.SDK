// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.SDK.DependencyInversion;
using Dolittle.SDK.Events.Processing;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Events.Handling.Builder
{
    /// <summary>
    /// Defines a system that can build and register an event handler.
    /// </summary>
    public interface ICanBuildAndRegisterAnEventHandler
    {
        /// <summary>
        /// Builds and registers the event handler.
        /// </summary>
        /// <param name="eventProcessors">The <see cref="IEventProcessors" />.</param>
        /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
        /// <param name="processingConverter">The <see cref="IEventProcessingConverter" />.</param>
        /// <param name="tenantScopedProvidersBuilder">The <see cref="TenantScopedProvidersBuilder"/>.</param>
        /// <param name="tenantScopedProvidersFactory">The <see cref="Func{TResult}"/> for getting <see cref="ITenantScopedProviders" />.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory" />.</param>
        /// <param name="cancelConnectToken">The <see cref="CancellationToken" />.</param>
        /// <param name="stopProcessingToken">The <see cref="CancellationToken"/> for stopping processing.</param>
        void BuildAndRegister(
            IEventProcessors eventProcessors,
            IEventTypes eventTypes,
            IEventProcessingConverter processingConverter,
            TenantScopedProvidersBuilder tenantScopedProvidersBuilder,
            Func<ITenantScopedProviders> tenantScopedProvidersFactory,
            ILoggerFactory loggerFactory,
            CancellationToken cancelConnectToken,
            CancellationToken stopProcessingToken);
    }
}
