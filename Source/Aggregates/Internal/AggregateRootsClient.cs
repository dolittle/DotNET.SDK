// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Aggregates.Contracts;
using Dolittle.SDK.Execution;
using Dolittle.SDK.Protobuf;
using Dolittle.SDK.Services;
using Dolittle.Services.Contracts;
using Microsoft.Extensions.Logging;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;

namespace Dolittle.SDK.Aggregates.Internal;

/// <summary>
/// Represents a system that knows how to register aggregate roots with the Runtime.
/// </summary>
public class AggregateRootsClient
{
    static readonly AggregateRootsRegisterAliasMethod _aliasMethod = new();
    readonly IPerformMethodCalls _caller;
    readonly ExecutionContext _executionContext;
    readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateRootsClient"/> class.
    /// </summary>
    /// <param name="caller">The method caller to use to perform calls to the Runtime.</param>
    /// <param name="executionContext">Tha base <see cref="ExecutionContext"/>.</param>
    /// <param name="logger">The <see cref="ILogger"/> to use.</param>
    public AggregateRootsClient(IPerformMethodCalls caller, ExecutionContext executionContext, ILogger logger)
    {
        _caller = caller;
        _executionContext = executionContext;
        _logger = logger;
    }

    /// <summary>
    /// Registers event types.
    /// </summary>
    /// <param name="aggregateRootTypes">The aggregate root types to register.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task Register(IEnumerable<AggregateRootType> aggregateRootTypes, CancellationToken cancellationToken)
        => Task.WhenAll(aggregateRootTypes.Select(CreateRequest).Select(_ => Register(_, cancellationToken)));

    AggregateRootAliasRegistrationRequest CreateRequest(AggregateRootType aggregateRootType)
    {
        var request = new AggregateRootAliasRegistrationRequest()
        {
            AggregateRoot = aggregateRootType.ToProtobuf(),
            CallContext = new CallRequestContext
            {
                ExecutionContext = _executionContext.ToProtobuf(),
                HeadId = HeadId.NotSet.ToProtobuf()
            }
        };
        if (aggregateRootType.HasAlias)
        {
            request.Alias = aggregateRootType.Alias;
        }
        return request;
    }

    async Task Register(AggregateRootAliasRegistrationRequest request, CancellationToken cancellationToken)
    {
        _logger.LogDebug(
            "Registering Aggregate Root {AggregateRoot} with Alias {Alias}",
            request.AggregateRoot.Id.ToGuid(),
            request.Alias);
        try
        {
            var response = await _caller.Call(_aliasMethod, request, cancellationToken).ConfigureAwait(false);
            if (response.Failure != null)
            {
                _logger.LogWarning(
                    "An error occurred while registering Aggregate Root {AggregateRoot} with Alias {Alias} because {Reason}",
                    request.AggregateRoot.Id.ToGuid(),
                    request.Alias,
                    response.Failure.Reason);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "An error occurred while registering Aggregate Root {AggregateRoot} with Alias {Alias}",
                request.AggregateRoot.Id.ToGuid(),
                request.Alias);
        }
    }
}
