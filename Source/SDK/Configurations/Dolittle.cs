// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.SDK.Configurations;

/// <summary>
/// Represents the configuration of the SDK.
/// </summary>
public class Dolittle
{
    /// <summary>
    /// The address of the Runtime to connect to.
    /// </summary>
    public Runtime Runtime { get; set; } = new();

    /// <summary>
    /// The version of the Head.
    /// </summary>
    public string? HeadVersion { get; set; }

    /// <summary>
    /// The ping interval to use for reverse call clients.
    /// </summary>
    public ushort? PingInterval { get; set; }

    /// <summary>
    /// How many seconds should the aggregates be kept in memory when not in use. -1 is forever, 0 will instantly remove them. 
    /// </summary>
    public int? AggregateIdleTimout { get; set; }

    /// <summary>
    /// The OpenTelemetry configuration.
    /// </summary>
    public OpenTelemetry? Otlp { get; set; }
}
