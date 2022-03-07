// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Dolittle.Benchmarks.Harness;

public class BoundPorts : IDisposable
{
    readonly (BoundPort boundHostPort, int containerPort)[] _boundPorts;

    public BoundPorts(params (BoundPort boundHostPort, int containerPort)[] boundPorts)
    {
        _boundPorts = boundPorts;
    }

    public IEnumerable<(BoundPort boundHostPort, int containerPort)> Ports => _boundPorts;

    public void Dispose()
    {
        foreach (var (boundPort, _) in _boundPorts)
        {
            boundPort?.Dispose();
        }
    }
}