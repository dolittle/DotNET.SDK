// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Builders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK;

/// <summary>
/// Represents an implementation of <see cref="IHostedService"/> for the <see cref="IDolittleClient"/>.
/// </summary>
public class DolittleClientService : IHostedService
{
    readonly IDolittleClient _dolittleClient;
    readonly DolittleClientConfiguration _clientConfiguration;
    ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DolittleClientService"/> class.
    /// </summary>
    /// <param name="dolittleClient">The <see cref="IDolittleClient"/>.</param>
    /// <param name="clientConfiguration">The <see cref="DolittleClientConfiguration"/>.</param>
    public DolittleClientService(IDolittleClient dolittleClient, DolittleClientConfiguration clientConfiguration)
    {
        _dolittleClient = dolittleClient;
        _clientConfiguration = clientConfiguration;
    }

    /// <inheritdoc />
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger = _clientConfiguration.LoggerFactory.CreateLogger<DolittleClientService>();
        _logger.ConnectingDolittleClient();
        try
        {
            await _dolittleClient.Connect(_clientConfiguration, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.ErrorWhileConnectingDolittleClient(ex);
            throw;
        }
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.DisconnectingDolittleClient();
        return _dolittleClient.Disconnect(cancellationToken);
    }
}
