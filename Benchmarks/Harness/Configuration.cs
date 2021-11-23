// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Docker.DotNet.Models;

namespace Dolittle.Benchmarks.Harness;

public static class Configuration
{
    public static IDictionary<string, IList<PortBinding>> CreatePortBindings(BoundPorts boundPorts)
        => CreatePortBindings(boundPorts.Ports.Select(_ => (_.boundHostPort.Port, _.containerPort)));

    public static IDictionary<string, IList<PortBinding>> CreatePortBindings(IEnumerable<(int hostPort, int containerPort)> bindings)
        => bindings.ToDictionary<(int hostPort, int containerPort), string, IList<PortBinding>>(
            binding => $"{binding.containerPort}/tcp",
            binding => new List<PortBinding>
            {
                new PortBinding
                {
                    HostPort = $"{binding.hostPort}",
                    HostIP = "localhost"
                }
            });
    
    
    public static IDictionary<string, EmptyStruct> CreateExposedPorts(BoundPorts boundPorts)
        => CreateExposedPorts(boundPorts.Ports.Select(_ => _.containerPort));

    public static IDictionary<string, EmptyStruct> CreateExposedPorts(IEnumerable<int> containerPorts)
        => containerPorts.ToDictionary(
            binding => $"{binding}/tcp",
            binding => new EmptyStruct());
}