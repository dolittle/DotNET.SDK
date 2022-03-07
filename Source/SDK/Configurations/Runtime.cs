// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#nullable enable
namespace Dolittle.SDK.Configurations;

/// <summary>
/// Represents the configuration for the address of the Runtime to connect to.
/// </summary>
public class Runtime
{
    /// <summary>
    /// The Runtime host to connect to.
    /// </summary>
    public string? Host { get; set; }
    
    /// <summary>
    /// The Runtime port to connect to.
    /// </summary>
    public ushort? Port { get; set; }
}
