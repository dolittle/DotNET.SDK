// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Docker.DotNet.Models;

namespace Dolittle.SDK.Benchmarks.Harness;

/// <summary>
/// Extension methods for <see cref="Docker"/> container status types.
/// </summary>
public static class DockerContainerExtensions
{
    public static bool IsNamed(this ContainerListResponse status, string name)
        => status.Names.Contains($"/{name}");
    
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
