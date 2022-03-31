// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Docker.DotNet.Models;

namespace Dolittle.SDK.Benchmarks.Harness;

/// <summary>
/// Extension methods for <see cref="Docker"/> container status types.
/// </summary>
public static class DockerContainerExtensions
{
    /// <summary>
    /// Checks whether a container has the specified name.
    /// </summary>
    /// <param name="status">The <see cref="ContainerListResponse"/> status of the container.</param>
    /// <param name="name">The name to check for.</param>
    /// <returns>True if the container has the specified name, false if not.</returns>
    public static bool IsNamed(this ContainerListResponse status, string name)
        => status.Names.Contains($"/{name}");
    
    /// <summary>
    /// Tries to get the published port number for a running container based on the exposed private port.
    /// </summary>
    /// <param name="status">The <see cref="ContainerListResponse"/> status of the container.</param>
    /// <param name="privatePort">The exposed private port number to find.</param>
    /// <param name="publicPort">The found published port number.</param>
    /// <returns>True if the port was found, false if not.</returns>
    public static bool TryGetPublishedPort(this ContainerListResponse status, ushort privatePort, out ushort publicPort)
    {
        foreach (var port in status.Ports)
        {
            if (port.PrivatePort == privatePort)
            {
                publicPort = port.PublicPort;
                return true;
            }
        }

        publicPort = 0;
        return false;
    }
}
