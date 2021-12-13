// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Handshake.Contracts;
using Dolittle.SDK.Failures;
using Dolittle.SDK.Protobuf;
using Dolittle.SDK.Services;
using Microsoft.Extensions.Logging;
using Version = Dolittle.SDK.Microservices.Version;

namespace Dolittle.SDK.Handshake.Internal;

/// <summary>
/// Represents a client for <see cref="Runtime.Handshake.Contracts.Handshake"/>.
/// </summary>
public class HandshakeClient : IPerformHandshake
{
    static readonly HandshakeMethod _method = new();
    readonly IPerformMethodCalls _caller;
    readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="HandshakeClient"/> class.
    /// </summary>
    /// <param name="caller">The method caller to use to perform calls to the Runtime.</param>
    /// <param name="logger">The <see cref="ILogger"/> to use.</param>
    public HandshakeClient(IPerformMethodCalls caller, ILogger logger)
    {
        _caller = caller;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<HandshakeResult> Perform(Version headVersion, CancellationToken cancellationToken)
    {
        try
        {
            var contractsVersion = Contracts.VersionInfo.CurrentVersion.ToVersion();
            var sdkVersion = VersionInfo.CurrentVersion;
            Log.PerformHandshake(_logger, headVersion, sdkVersion, contractsVersion);
            var request = new HandshakeRequest
            {
                Sdk = "DotNET",
                ContractsVersion = contractsVersion.ToProtobuf(),
                SdkVersion = sdkVersion.ToProtobuf(),
                HeadVersion = headVersion.ToProtobuf()
            };
            var response = await _caller.Call(_method, request, cancellationToken).ConfigureAwait(false);
            if (response.Failure != null)
            {
                var failure = new Failure(response.Failure.Id.ToGuid(), response.Failure.Reason);
                Log.HandshakeFailedResponse(_logger, failure.Reason, failure.Id);
                throw new DolittleRuntimeFailedHandshake(failure);
            }

            Log.SuccessfullyPerformedHandshake(_logger, headVersion, sdkVersion, contractsVersion, response.RuntimeVersion.ToVersion(), response.ContractsVersion.ToVersion());
            return new HandshakeResult(response.MicroserviceId.ToGuid(), response.Environment);
        }
        catch (Exception ex)
        {
            Log.ErrorPerformingHandshake(_logger, ex);
            throw;
        }
    }
}
