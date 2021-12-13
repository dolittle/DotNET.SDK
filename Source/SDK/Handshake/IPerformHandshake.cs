// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Microservices;

namespace Dolittle.SDK.Handshake;

/// <summary>
/// Defines a system that can perform the handshake with the Dolittle Runtime.
/// </summary>
public interface IPerformHandshake
{
    /// <summary>
    /// Performs the handshake with the Dolittle Runtime.
    /// </summary>
    /// <param name="headVersion">The <see cref="Version"/> of the Head.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task{TResult}"/>that, when resolved, returns the <see cref="HandshakeResult"/>.</returns>
    Task<HandshakeResult> Perform(Version headVersion, CancellationToken cancellationToken);
}
