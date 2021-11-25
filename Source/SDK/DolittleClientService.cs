// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Dolittle.SDK
{
    /// <summary>
    /// Represents an implementation of <see cref="IHostedService"/> for the <see cref="IDolittleClient"/>.
    /// </summary>
    public class DolittleClientService : IHostedService
    {
        readonly IDolittleClient _dolittleClient;
        readonly DolittleClientConfiguration _dolittleConfigurationAccessor;
        ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DolittleClientService"/> class.
        /// </summary>
        /// <param name="dolittleClient">The <see cref="IDolittleClient"/>.</param>
        /// <param name="dolittleConfigurationAccessor">The <see cref="IOptions{TOptions}"/> of <see cref="DolittleClientConfiguration"/>.</param>
        public DolittleClientService(IDolittleClient dolittleClient, IOptions<DolittleClientConfiguration> dolittleConfigurationAccessor)
        {
            _dolittleClient = dolittleClient;
            _dolittleConfigurationAccessor = dolittleConfigurationAccessor.Value;
        }

        /// <inheritdoc />
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger = _dolittleConfigurationAccessor.LoggerFactory.CreateLogger<DolittleClientService>();
            _logger.LogInformation("Connecting Dolittle Client");
            await _dolittleClient.Connect(_dolittleConfigurationAccessor, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Disconnecting Dolittle Client");
            return _dolittleClient.Disconnect(cancellationToken);
        }
    }
}