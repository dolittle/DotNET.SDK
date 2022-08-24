// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.SDK.Common.Model;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Common.ClientSetup;

/// <summary>
/// Defines a system that keeps track of the results of building a client.
/// </summary>
public interface IClientBuildResults
{
    /// <summary>
    /// Gets all <see cref="ClientBuildResult"/>.
    /// </summary>
    IEnumerable<ClientBuildResult> All { get; }

    /// <summary>
    /// Gets a value indicating whether the client building failed or not.
    /// </summary>
    bool Failed { get; }

    /// <summary>
    /// Gets all <see cref="IdentifiableClientBuildResult"/> for the specific <see cref="IIdentifier"/>.
    /// </summary>
    IEnumerable<IdentifiableClientBuildResult> GetFor<TId>()
        where TId : class, IIdentifier;

    /// <summary>
    /// Adds the <see cref="ClientBuildResult"/>.
    /// </summary>
    /// <param name="result">The <see cref="ClientBuildResult"/>.</param>
    void Add(ClientBuildResult result);
    
    /// <summary>
    /// Adds an information build result.
    /// </summary>
    /// <param name="message">The information message</param>
    void AddInformation(string message);

    /// <summary>
    /// Adds an information build result.
    /// </summary>
    /// <param name="id">The <see cref="IIdentifier"/>.</param>
    /// <param name="alias">The alias.</param>
    /// <param name="message">The information message</param>
    void AddInformation(IIdentifier id, string alias, string message);

    /// <summary>
    /// Adds a failure build result.
    /// </summary>
    /// <param name="message">The failure message.</param>
    /// <param name="fix">The optional suggested fix to resolve the failure.</param>
    void AddFailure(string message, string fix = "");

    /// <summary>
    /// Adds a failure build result.
    /// </summary>
    /// <param name="id">The <see cref="IIdentifier"/>.</param>
    /// <param name="alias">The alias.</param>
    /// <param name="message">The failure message.</param>
    /// <param name="fix">The optional suggested fix to resolve the failure.</param>
    void AddFailure(IIdentifier id, string alias, string message, string fix = "");

    /// <summary>
    /// Adds an error build result.
    /// </summary>
    /// <param name="error">The <see cref="Exception"/> that was thrown.</param>
    void AddError(Exception error);

    /// <summary>
    /// Adds an error build result.
    /// </summary>
    /// <param name="id">The <see cref="IIdentifier"/>.</param>
    /// <param name="alias">The alias.</param>
    /// <param name="error">The <see cref="Exception"/> that was thrown.</param>
    void AddError(IIdentifier id, string alias, Exception error);

    /// <summary>
    /// Writes the build result to the provided logger.
    /// </summary>
    /// <param name="logger">The provided <see cref="ILogger"/>.</param>
    public void WriteTo(ILogger logger);
}
