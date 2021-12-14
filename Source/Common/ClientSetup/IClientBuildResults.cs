// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


using System;

namespace Dolittle.SDK.Common.ClientSetup;

/// <summary>
/// Defines a system that keeps track of the results of building a client.
/// </summary>
public interface IClientBuildResults
{
    /// <summary>
    /// Adds an information build result.
    /// </summary>
    /// <param name="message">The information message</param>
    void AddInformation(string message);

    /// <summary>
    /// Adds a failure build result.
    /// </summary>
    /// <param name="message">The failure message.</param>
    /// <param name="fix">The optional suggested fix to resolve the failure.</param>
    void AddFailure(string message, string fix = "");

    /// <summary>
    /// Adds an error build result.
    /// </summary>
    /// <param name="error">The <see cref="Exception"/> that was thrown.</param>
    void AddError(Exception error);
    
    /// <summary>
    /// Gets a value indicating whether the client building failed or not.
    /// </summary>
    bool Failed { get; }
}
