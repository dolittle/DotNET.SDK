// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Dolittle.Benchmarks.Harness;

public class OpenPortPool
{
    readonly object _findLock = new(); 
    readonly HashSet<int> _usedPorts = new();

    public BoundPort Find()
    {
        while (true)
        {
            var l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            var port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            if (_usedPorts.Contains(port))
            {
                continue;
            }
            lock (_findLock)
            {
                if (_usedPorts.Contains(port))
                {
                    continue;
                }
                _usedPorts.Add(port);
                return new BoundPort(port, Free);
            }
        }
    }

    public BoundPort Claim(int port)
    {
        if (_usedPorts.Contains(port))
        {
            throw new ArgumentException($"Port {port} is already in use");
        }

        return new BoundPort(port, Free);
    }

    void Free(int i) => _usedPorts.Remove(i);


}