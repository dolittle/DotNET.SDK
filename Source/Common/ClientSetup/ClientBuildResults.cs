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
    readonly List<ClientBuildResult> _results = [];
    readonly List<IdentifiableClientBuildResult> _identifiableResults = [];

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
        => Add(new IdentifiableClientBuildResult(id, ClientBuildResult.Information(message)));

    /// <inheritdoc />
    public void AddFailure(string message, string fix = "")
        => Add(ClientBuildResult.Failure(message, fix));

    /// <inheritdoc />
    public void AddFailure(IIdentifier id, string message, string fix = "")
        => Add(new IdentifiableClientBuildResult(id, ClientBuildResult.Failure(message, fix)));

    /// <inheritdoc />
    public void AddError(Exception error)
        => Add(ClientBuildResult.Error(error));

    /// <inheritdoc />
    public void AddError(IIdentifier id, Exception error)
        => Add(new IdentifiableClientBuildResult(id, ClientBuildResult.Error(error)));

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

    public string ErrorString => !Failed
        ? ""
        : string.Join("\n", _results.Where(_ => _.IsFailed).Select(_ => _.Message)) +
          string.Join("\n", _identifiableResults.Where(_ => _.Result.IsFailed).Select(_ => _.Result.Message));

    void Add(IdentifiableClientBuildResult result)
    {
        if (result.Result.IsFailed)
        {
            Failed = true;
        }

        _identifiableResults.Add(result);
    }
}
