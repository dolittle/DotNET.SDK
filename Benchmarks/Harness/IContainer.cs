// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;

namespace Dolittle.Benchmarks.Harness;

public interface IContainer : IAsyncDisposable
{
    Task Start();
    Task Stop();
}
