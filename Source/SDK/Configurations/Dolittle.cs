// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Configurations;

public class Runtime
{
    public string? Host { get; set; }
    public ushort? Port { get; set; }
}

public class Dolittle
{
    public Runtime Runtime { get; set; }
    public string? HeadVersion { get; set; }
    public ushort? PingInterval { get; set; }
}
