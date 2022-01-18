// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Services;

/// <summary>
/// Exception that gets thrown when not the Client is not able to connect to a Runtime.
/// </summary>
public class CouldNotConnectToRuntime : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CouldNotConnectToRuntime"/> class.
    /// </summary>
    /// <param name="host">The host the Client attempted to connect to.</param>
    /// <param name="port">The port the Client attempted to connect to.</param>
    public CouldNotConnectToRuntime(string host, ushort port)
        : base($"Could not connect to a Runtime on '{host}:{port}'. Please make sure a Runtime is running, and that the private port (usually 50053) is accessible on the specified port.")
    {
    }
}