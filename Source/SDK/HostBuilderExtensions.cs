// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Hosting;

namespace Dolittle.SDK
{
    /// <summary>
    /// Static extension methods on the <see cref="IHostBuilder"/> for setting up Dolittle.
    /// </summary>
    public static class HostBuilderExtensions
    {
        /// <summary>
        /// Uses Dolittle by setting it up with the <see cref="IHostBuilder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IHostBuilder"/>.</param>
        /// <param name="configureSetup">The <see cref="SetupDolittleClient"/> callback for configuring the <see cref="DolittleClientBuilder"/>.</param>
        /// <param name="configureClientConfiguration">The <see cref="ConfigureDolittleClient"/> callback for configuring the <see cref="DolittleClientConfiguration"/>.</param>
        /// <returns>The builder for continuation.</returns>
        public static IHostBuilder UseDolittle(this IHostBuilder builder, SetupDolittleClient configureSetup, ConfigureDolittleClient configureClientConfiguration)
            => builder.ConfigureServices(services => services.AddDolittle(configureSetup, configureClientConfiguration));
    }
}