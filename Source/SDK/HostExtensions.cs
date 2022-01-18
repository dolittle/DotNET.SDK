// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Builders;
using Microsoft.Extensions.Hosting;

namespace Dolittle.SDK;

/// <summary>
/// Static extension methods on the <see cref="IHostBuilder"/> for setting up Dolittle.
/// </summary>
public static class HostExtensions
{
    /// <summary>
    /// Uses Dolittle by setting it up with the <see cref="IHostBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IHostBuilder"/>.</param>
    /// <param name="configureSetup">The optional <see cref="SetupDolittleClient"/> callback for configuring the <see cref="SetupBuilder"/>.</param>
    /// <param name="configureClientConfiguration">The optional <see cref="ConfigureDolittleClient"/> callback for configuring the <see cref="DolittleClientConfiguration"/>.</param>
    /// <returns>The builder for continuation.</returns>
    public static IHostBuilder UseDolittle(this IHostBuilder builder, SetupDolittleClient configureSetup = default, ConfigureDolittleClient configureClientConfiguration = default)
        => builder.ConfigureServices(services => services.AddDolittle(configureSetup, configureClientConfiguration));

    /// <summary>
    /// Gets a connected <see cref="IDolittleClient"/> from the <see cref="IHost"/> <see cref="IHost.Services"/>.
    /// If the <see cref="IDolittleClient"/> has not been connected the connection will be established.
    /// </summary>
    /// <param name="host">The <see cref="IHost"/>.</param>
    /// <param name="configureClient">The optional <see cref="ConfigureDolittleClient"/> callback.</param>
    /// <param name="cancellationToken">The optional <see cref="CancellationToken"/> used if the <see cref="IDolittleClient"/> needs to connect.</param>
    /// <returns>A <see cref="Task{TResult}"/> that, when resolved, returns the connected <see cref="IDolittleClient"/>.</returns>
    public static Task<IDolittleClient> GetDolittleClient(this IHost host, ConfigureDolittleClient configureClient = default, CancellationToken cancellationToken = default)
        => host.Services.GetDolittleClient(configureClient, cancellationToken);
}
