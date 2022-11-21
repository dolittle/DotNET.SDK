// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.SDK.Common.Model;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Common.ClientSetup;

/// <summary>
/// Represents an implementation of <see cref="IClientBuildResults"/>.
/// </summary>
public class ClientBuildResults : IClientBuildResults
{
    readonly List<ClientBuildResult> _results = new();
    readonly List<IdentifiableClientBuildResult> _identifiableResults = new();

    /// <inheritdoc />
    public IEnumerable<ClientBuildResult> AllNonIdentifiable => _results;

    /// <inheritdoc />
    public IEnumerable<IdentifiableClientBuildResult> GetFor<TId>()
        where TId : class, IIdentifier
        => _identifiableResults.Where(_ => _.Identifier is TId);

    /// <inheritdoc />
    public void Add(ClientBuildResult result)
    {
        if (result.IsFailed)
        {
            Failed = true;
        }
        _results.Add(result);
    }

    /// <inheritdoc />
    public void AddInformation(string message)
        => Add(ClientBuildResult.Information(message));

    /// <inheritdoc />
    public void AddInformation(IIdentifier id, string message)
        => _identifiableResults.Add(new IdentifiableClientBuildResult(id, ClientBuildResult.Information(message)));

    /// <inheritdoc />
    public void AddFailure(string message, string fix = "")
        => Add(ClientBuildResult.Failure(message, fix));

    /// <inheritdoc />
    public void AddFailure(IIdentifier id, string message, string fix = "")
        => _identifiableResults.Add(new IdentifiableClientBuildResult(id, ClientBuildResult.Failure(message, fix)));

    /// <inheritdoc />
    public void AddError(Exception error)
        => Add(ClientBuildResult.Error(error));

    /// <inheritdoc />
    public void AddError(IIdentifier id,  Exception error)
        => _identifiableResults.Add(new IdentifiableClientBuildResult(id, ClientBuildResult.Error(error)));

    /// <inheritdoc />
    public bool Failed { get; private set; }

    /// <inheritdoc />
    public void WriteTo(ILogger logger)
    {
        foreach (var result in _results)
        {
            result.Log(logger);
        }
        
        foreach (var group in _identifiableResults.GroupBy(_ => _.Identifier))
        {
            foreach (var result in group)
            {
                result.Log(logger);
            }
        }
    }
}
