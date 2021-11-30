// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Dolittle.SDK
{
    /// <summary>
    /// Extension method for <see cref="IServiceProvider"/> for getting a connected Dolittle client.
    /// </summary>
    public static class ServiceProviderExtensions
    {
        /// <summary>
        /// Gets a connected <see cref="IDolittleClient"/> from the <see cref="IServiceProvider"/>.
        /// If the <see cref="IDolittleClient"/> has not been connected the connection will be established.
        /// </summary>
        /// <param name="provider">The <see cref="IServiceProvider"/>.</param>
        /// <param name="configureClient">The optional <see cref="ConfigureDolittleClient"/> callback.</param>
        /// <param name="cancellationToken">The optional <see cref="CancellationToken"/> used if the <see cref="IDolittleClient"/> needs to connect.</param>
        /// <returns>A <see cref="Task{TResult}"/> that, when resolved, returns the connected <see cref="IDolittleClient"/>.</returns>
        public static Task<IDolittleClient> GetDolittleClient(this IServiceProvider provider, ConfigureDolittleClient configureClient = default,  CancellationToken cancellationToken = default)
        {
            var client = provider.GetService<IDolittleClient>() ?? throw new DolittleClientNotSetup();
            if (client.Connected)
            {
                return Task.FromResult(client);
            }

            var clientConfig = provider.GetService<IOptions<DolittleClientConfiguration>>()?.Value ?? new DolittleClientConfiguration();
            configureClient?.Invoke(clientConfig);
            return client.Connect(clientConfig, cancellationToken);
        }
    }
}