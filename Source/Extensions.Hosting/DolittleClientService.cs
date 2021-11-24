// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Tenancy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Extensions.Hosting
{
    /// <summary>
    /// Represents an implementation of <see cref="IHostedService"/> for the <see cref="IDolittleClient"/>.
    /// </summary>
    public class DolittleClientService : IHostedService
    {
        readonly IServiceProvider _provider;
        readonly IDolittleClient _dolittleClient;
        readonly Action<DolittleClientConfigurationBuilder> _configureClientConfiguration;
        readonly Action<TenantId, IServiceCollection> _configureTenantContainers;
        readonly IHostEnvironment _hostEnvironment;

        /// <summary>
        /// Initializes a new instance of the <see cref="DolittleClientService"/> class.
        /// </summary>
        /// <param name="provider">The root <see cref="IServiceProvider"/>.</param>
        /// <param name="dolittleClient">The <see cref="IDolittleClient"/>.</param>
        /// <param name="hostEnvironment">The <see cref="IHostEnvironment"/>.</param>
        /// <param name="configureClientConfiguration">The <see cref="Action"/> callback for configuring the <see cref="DolittleClientConfigurationBuilder"/>.</param>
        /// <param name="configureTenantContainers">The <see cref="Action"/> callback for configuring the IoC Containers for each tenant.</param>
        public DolittleClientService(
            IServiceProvider provider,
            IDolittleClient dolittleClient,
            IHostEnvironment hostEnvironment,
            Action<DolittleClientConfigurationBuilder> configureClientConfiguration,
            Action<TenantId, IServiceCollection> configureTenantContainers)
        {
            _provider = provider;
            _dolittleClient = dolittleClient;
            _configureClientConfiguration = configureClientConfiguration;
            _configureTenantContainers = configureTenantContainers;
            _hostEnvironment = hostEnvironment;
        }

        /// <inheritdoc />
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _dolittleClient.Connect(
                _ =>
                {
                    _configureClientConfiguration?.Invoke(_);
                    _
                        .WithCancellation(cancellationToken)
                        .WithLogging(_provider.GetRequiredService<ILoggerFactory>());
                },
                _hostEnvironment.IsDevelopment() ? new DolittleClientConnectionArguments() : new ContextFromPlatformResolver() as ICanResolveExecutionContextForDolittleClient).ConfigureAwait(false);
            _ = _dolittleClient
                .WithServiceProvider(_provider)
                .WithTenantServices(_configureTenantContainers)
                .Start();
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}