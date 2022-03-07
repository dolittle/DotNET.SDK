// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Benchmarks.Harness;

public class BoundPort : IDisposable
{
    readonly Action<int> _free;

    public BoundPort(int port, Action<int> free)
    {
        _free = free;
        Port = port;
    }
    
    public int Port { get; }

    public void Dispose() => _free(Port);
}