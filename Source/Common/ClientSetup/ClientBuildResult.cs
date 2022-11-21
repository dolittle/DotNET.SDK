// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Common.Model;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Common.ClientSetup;

/// <summary>
/// Represents a client build result.
/// </summary>
public abstract class ClientBuildResult
{
    readonly Exception _error;
    readonly LogLevel _logLevel;

    /// <summary>
    /// Initializes an instance of the <see cref="ClientBuildResult"/> class.
    /// </summary>
    /// <param name="logLevel">The <see cref="LogLevel"/>.</param>
    /// <param name="message">The build message.</param>
    /// <param name="failed">The value indicating whether this is a failed result.</param>
    /// <param name="error">The optional error <see cref="Exception"/>.</param>
    protected ClientBuildResult(LogLevel logLevel, string message, bool failed, Exception error = default)
    {
        _logLevel = logLevel;
        Message = message;
        _error = error;
        IsFailed = failed;
    }

    /// <summary>
    /// Creates an <see cref="InformationBuildResult"/>.
    /// </summary>
    /// <param name="message">The build message.</param>
    /// <returns>The <see cref="InformationBuildResult"/>.</returns>
    public static InformationBuildResult Information(string message) => new(message);
    
    /// <summary>
    /// Creates an <see cref="FailureBuildResult"/>.
    /// </summary>
    /// <param name="message">The build message.</param>
    /// <param name="fix">The fix message.</param>
    /// <returns>The <see cref="FailureBuildResult"/>.</returns>
    public static FailureBuildResult Failure(string message, string fix = default) => new(message, fix);
    
    /// <summary>
    /// Creates an <see cref="ErrorBuildResult"/>.
    /// </summary>
    /// <param name="error">The error <see cref="Exception"/>.</param>
    /// <returns>The <see cref="ErrorBuildResult"/>.</returns>
    public static ErrorBuildResult Error(Exception error) => new(error);
    
    
    /// <summary>
    /// Gets the message.
    /// </summary>
    public string Message { get; }
    
    /// <summary>
    /// Gets a value indicating whether this is a failed result.
    /// </summary>
    public bool IsFailed { get; }


#pragma warning disable CA1848
#pragma warning disable CA2254
    /// <summary>
    /// Logs the build message.
    /// </summary>
    /// <param name="logger">The provided <see cref="ILogger"/>to log the message to.</param>
    public void Log(ILogger logger)
        // ReSharper disable once TemplateIsNotCompileTimeConstantProblem
        => logger.Log(_logLevel, 0, _error, Message);
    
    /// <summary>
    /// Logs the build message.
    /// </summary>
    /// <param name="logger">The provided <see cref="ILogger"/>to log the message to.</param>
    public void Log(ILogger logger, IIdentifier identifier)
        // ReSharper disable once TemplateIsNotCompileTimeConstantProblem
        => logger.Log(_logLevel, 0, _error, $"{identifier}\n\t{Message}");
#pragma warning restore CA2254
#pragma warning restore CA1848
}
