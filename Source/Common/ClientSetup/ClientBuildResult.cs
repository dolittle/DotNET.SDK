// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Common.ClientSetup;

/// <summary>
/// Represents a client build result.
/// </summary>
public class ClientBuildResult
{
    readonly string _message;
    readonly Exception _error;
    readonly LogLevel _logLevel;

    ClientBuildResult(LogLevel logLevel, string message, Exception error = default)
    {
        _logLevel = logLevel;
        _message = message;
        _error = error;
    }

    public static ClientBuildResult Information(string message)
        => new(LogLevel.Debug, message);

    public static ClientBuildResult Failure(string message, string fix = default)
        => new(LogLevel.Warning, string.IsNullOrEmpty(fix) ? message : $"{message}. {fix}");
    
    public static ClientBuildResult Error(Exception error)
        => new(LogLevel.Error, error.Message, error);


#pragma warning disable CA1848
#pragma warning disable CA2254
    /// <inheritdoc />
    public void Log(ILogger logger)
        // ReSharper disable once TemplateIsNotCompileTimeConstantProblem
        => logger.Log(_logLevel, 0, _error, _message);
#pragma warning restore CA2254
#pragma warning restore CA1848
}
