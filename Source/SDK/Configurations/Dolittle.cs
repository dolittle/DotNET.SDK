// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#nullable enable
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

    public OtlpConfig? Otlp { get; set; }
}
