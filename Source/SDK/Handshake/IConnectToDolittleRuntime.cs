// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;

namespace Dolittle.SDK.Handshake;

/// <summary>
/// Defines a system that can perform an handshake with the Dolittle Runtime and connect to it.
/// </summary>
public interface IConnectToDolittleRuntime
{
    /// <summary>
    /// Tries to connect to the Dolittle Runtime forever.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> for stopping the connect attempts.</param>
    /// <returns>A <see cref="Task{TResult}"/> that, when resolved, returns the <see cref="ConnectionResult"/>.</returns>
    Task<ConnectionResult> ConnectForever(CancellationToken cancellationToken);
}