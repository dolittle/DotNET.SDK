// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Common.ClientSetup;
using Version = Dolittle.SDK.Microservices.Version;

namespace Dolittle.SDK.Handshake;

/// <summary>
/// Defines a system that can perform the handshake with the Dolittle Runtime.
/// </summary>
public interface IPerformHandshake
{
    /// <summary>
    /// Performs the handshake with the Dolittle Runtime.
    /// </summary>
    /// <param name="attemptNum">The attempt number.</param>
    /// <param name="timeSpent">The time spent completing handshake.</param>
    /// <param name="headVersion">The <see cref="Version"/> of the Head.</param>
    /// <param name="buildResults">The <see cref="IClientBuildResults"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task{TResult}"/>that, when resolved, returns the <see cref="HandshakeResult"/>.</returns>
    Task<HandshakeResult> Perform(uint attemptNum, TimeSpan timeSpent, Version headVersion, IClientBuildResults buildResults, CancellationToken cancellationToken);
}
