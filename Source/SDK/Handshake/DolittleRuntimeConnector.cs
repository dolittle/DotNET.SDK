// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Execution;
using Dolittle.SDK.Security;
using Dolittle.SDK.Services;
using Dolittle.SDK.Tenancy;
using Dolittle.SDK.Tenancy.Client;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;
using Version = Dolittle.SDK.Microservices.Version;

namespace Dolittle.SDK.Handshake;

/// <summary>
/// Represents an implementation of <see cref="IConnectToDolittleRuntime"/>.
/// </summary>
public class DolittleRuntimeConnector : IConnectToDolittleRuntime
{
    static readonly TimeSpan _waitMaxTime = TimeSpan.FromSeconds(5);
    readonly string _runtimeHost;
    readonly ushort _runtimePort;
    readonly Version _headVersion;
    readonly IPerformHandshake _handshakePerformer;
    readonly ITenants _tenants;
    readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DolittleRuntimeConnector"/> class.
    /// </summary>
    /// <param name="runtimeHost">The Dolittle Runtime host.</param>
    /// <param name="runtimePort">The Dolittle Runtime port.</param>
    /// <param name="headVersion">The <see cref="Version"/> of the Head.</param>
    /// <param name="handshakePerformer">The <see cref="IPerformHandshake"/>.</param>
    /// <param name="tenants">The <see cref="ITenants"/>.</param>
    /// <param name="logger">The <see cref="ILogger"/>.</param>
    public DolittleRuntimeConnector(
        string runtimeHost,
        ushort runtimePort,
        Version headVersion,
        IPerformHandshake handshakePerformer,
        ITenants tenants,
        ILogger logger)
    {
        _runtimeHost = runtimeHost;
        _runtimePort = runtimePort;
        _headVersion = headVersion;
        _handshakePerformer = handshakePerformer;
        _tenants = tenants;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<ConnectionResult> ConnectForever(CancellationToken cancellationToken)
    {
        var (executionContext, handshakeResult) = await TryPerformHandshakeForever(cancellationToken).ConfigureAwait(false);
        return new ConnectionResult(executionContext, await _tenants.GetAll(executionContext, cancellationToken).ConfigureAwait(false), handshakeResult.OTLPEndpoint);
    }

    static TimeSpan GetTimeToWait(TimeSpan current)
        => TimeSpan.FromMilliseconds(Math.Min(current.TotalMilliseconds, _waitMaxTime.TotalMilliseconds));

    ExecutionContext CreateExecutionContextFromHandshake(HandshakeResult result)
        => new(
            result.MicroserviceId,
            TenantId.System,
            _headVersion,
            result.Environment,
            CorrelationId.System,
            Claims.Empty,
            CultureInfo.InvariantCulture,
            null);

    async Task<(ExecutionContext, HandshakeResult)> TryPerformHandshakeForever(CancellationToken cancellationToken)
    {
        var currentWaitTime = GetTimeToWait(TimeSpan.FromSeconds(1));
        var startTime = DateTimeOffset.Now;
        uint attempts = 1;
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                _logger.ConnectingToDolittleRuntime();
                var handshakeResult = await _handshakePerformer.Perform(attempts++, DateTimeOffset.Now - startTime, _headVersion, cancellationToken).ConfigureAwait(false);
                return (CreateExecutionContextFromHandshake(handshakeResult), handshakeResult);
            }
            catch (RpcException ex) when (ex.Status.StatusCode == StatusCode.Unimplemented)
            {
                throw new RuntimeCannotPerformHandshake(_runtimeHost, _runtimePort);
            }
            catch (Exception ex) when (ex is not DolittleRuntimeFailedHandshake)
            {
                _logger.RetryConnect(currentWaitTime, ex.Message);
                await Task.Delay(currentWaitTime, cancellationToken).ConfigureAwait(false);
                currentWaitTime = GetTimeToWait(currentWaitTime * 2);
            }
        }

        throw new CouldNotConnectToRuntime(_runtimeHost, _runtimePort);
    }
}
