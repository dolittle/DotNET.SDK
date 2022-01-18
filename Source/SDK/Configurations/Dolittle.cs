// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.SDK.Configurations;

public record Runtime(string Host, ushort Port);

public record Dolittle(Runtime Runtime, uint PingInterval, string Version);
