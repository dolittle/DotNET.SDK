// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Handshake;

/// <summary>
/// Exception that gets thrown when connecting to a Dolittle Runtime that doesn't support handshakes.
/// </summary>
public class RuntimeCannotPerformHandshake : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RuntimeCannotPerformHandshake"/> class.
    /// </summary>
    /// <param name="runtimeHost">The configured Runtime host.</param>
    /// <param name="runtimePort">The configured Runtime port.</param>
    public RuntimeCannotPerformHandshake(string runtimeHost, ushort runtimePort)
        : base($"This version of the SDK {VersionInfo.CurrentVersion} is not compatible with the Dolittle Runtime you are connecting to on {runtimeHost}:{runtimePort}. Please upgrade the Runtime to version 7.4.0 or later.")
    {
    }
}
