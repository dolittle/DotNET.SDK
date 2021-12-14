// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Common.ClientSetup;

/// <summary>
/// Represents an implementation of <see cref="IClientBuildResults"/>.
/// </summary>
public class ClientBuildResults : IClientBuildResults
{
    readonly List<ClientBuildResult> _results = new();

    /// <inheritdoc />
    public void AddInformation(string message)
        => _results.Add(ClientBuildResult.Information(message));

    /// <inheritdoc />
    public void AddFailure(string message, string fix = "")
    {
        Failed = true;
        _results.Add(ClientBuildResult.Failure(message, fix));
    }

    /// <inheritdoc />
    public void AddError(Exception error)
    {
        Failed = true;
        _results.Add(ClientBuildResult.Error(error));
    }

    /// <inheritdoc />
    public bool Failed { get; private set; }

    /// <summary>
    /// Writes the build result to the provided logger.
    /// </summary>
    /// <param name="logger">The provided <see cref="ILogger"/>.</param>
    public void WriteTo(ILogger logger)
    {
        foreach (var result in _results)
        {
            result.Log(logger);
        }
    }
}
