// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Dolittle.SDK.Extensions.Hosting
{
    public static class WebHostBuilderExtensions
    {
        public IHostBuilder UseDolittle(this IHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Register hosted service
            });

            return builder;
        }
    }

    public class DolittleHostedService : IHostedService
    {
        public Task StartAsync(CancellationToken cancellationToken)
            => throw new System.NotImplementedException();

        public Task StopAsync(CancellationToken cancellationToken)
            => throw new System.NotImplementedException();
    }
}