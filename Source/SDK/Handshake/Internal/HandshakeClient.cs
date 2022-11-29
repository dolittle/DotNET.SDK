// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Artifacts.Contracts;
using Dolittle.Runtime.Client.Contracts;
using Dolittle.Runtime.Handshake.Contracts;
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Artifacts;
using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Common.Model;
using Dolittle.SDK.Embeddings;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Filters;
using Dolittle.SDK.Events.Handling;
using Dolittle.SDK.Failures;
using Dolittle.SDK.Projections;
using Dolittle.SDK.Protobuf;
using Dolittle.SDK.Services;
using Google.Protobuf.WellKnownTypes;
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
    public async Task<HandshakeResult> Perform(uint attemptNum, TimeSpan timeSpent, Version headVersion, IClientBuildResults buildResults, CancellationToken cancellationToken)
    {
        try
        {
            var contractsVersion = Contracts.VersionInfo.CurrentVersion.ToVersion();
            var sdkVersion = VersionInfo.CurrentVersion;
            _logger.PerformHandshake(headVersion, sdkVersion, contractsVersion);
            var request = new HandshakeRequest
            {
                SdkIdentifier = "DotNET",
                ContractsVersion = contractsVersion.ToProtobuf(),
                SdkVersion = sdkVersion.ToProtobuf(),
                HeadVersion = headVersion.ToProtobuf(),
                Attempt = attemptNum,
                TimeSpent = timeSpent.ToDuration(),
                BuildResults = ToProtobuf(buildResults)
            };
            var response = await _caller.Call(_method, request, cancellationToken).ConfigureAwait(false);
            if (response.Failure != null)
            {
                var failure = new Failure(response.Failure.Id.ToGuid(), response.Failure.Reason);
                _logger.HandshakeFailedResponse(failure.Reason, failure.Id);
                throw new DolittleRuntimeFailedHandshake(failure);
            }

            _logger.SuccessfullyPerformedHandshake(headVersion, sdkVersion, contractsVersion, response.RuntimeVersion.ToVersion(), response.ContractsVersion.ToVersion());
            Uri? otlpEndpoint = null;
            if (response.HasOtlpEndpoint)
            {
                if (!Uri.TryCreate(response.OtlpEndpoint, UriKind.Absolute, out otlpEndpoint))
                {
                    _logger.InvalidOtlpEndpoint(response.OtlpEndpoint);
                }
            }
            
            return new HandshakeResult(response.MicroserviceId.ToGuid(), response.EnvironmentName, otlpEndpoint);
        }
        catch (Exception ex)
        {
            _logger.ErrorPerformingHandshake(ex);
            throw;
        }
    }

    static BuildResults ToProtobuf(IClientBuildResults buildResults)
    {
        var protobuf = new BuildResults();
        protobuf.AggregateRoots.AddRange(GetBuildResultsFor<AggregateRootType>(buildResults, _ => _.Generation));
        protobuf.EventTypes.AddRange(GetBuildResultsFor<EventType>(buildResults, _ => _.Generation));
        protobuf.EventHandlers.AddRange(GetBuildResultsFor<EventHandlerModelId>(buildResults, _ => 0));
        protobuf.Filters.AddRange(GetBuildResultsFor<FilterModelId>(buildResults, _ => 0));
        protobuf.Projections.AddRange(GetBuildResultsFor<ProjectionModelId>(buildResults, _ => 0));
        protobuf.Embeddings.AddRange(GetBuildResultsFor<EmbeddingModelId>(buildResults, _ => 0));
        protobuf.Other.AddRange(buildResults.AllNonIdentifiable.Select(_ => _.ToProtobuf()));
        return protobuf;
    }

    static IEnumerable<ArtifactBuildResult> GetBuildResultsFor<TIdentifier>(IClientBuildResults buildResults, Func<TIdentifier, Generation> getGeneration)
        where TIdentifier : class, IIdentifier
        => buildResults.GetFor<TIdentifier>().Select(_ => ToProtobuf(_, getGeneration));

    static ArtifactBuildResult ToProtobuf<TIdentifier>(IdentifiableClientBuildResult buildResult, Func<TIdentifier, Generation> getGeneration)
        where TIdentifier : class, IIdentifier
    {
        var result = new ArtifactBuildResult
        {
            Aritfact = new Artifact
            {
                Id = buildResult.Identifier.Id.ToProtobuf(),
                Generation = getGeneration((buildResult.Identifier as TIdentifier)!).Value
            },
            BuildResult = buildResult.Result.ToProtobuf()
        };
        if (!string.IsNullOrEmpty(buildResult.Identifier.Alias))
        {
            result.Alias = buildResult.Identifier.Alias;
        }

        return result;
    }

}
