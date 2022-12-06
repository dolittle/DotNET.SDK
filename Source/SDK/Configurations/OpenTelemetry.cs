// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.SDK.Configurations;

/// <summary>
/// Represents the configuration for OpenTelemetry for the Client.
/// </summary>
public class OpenTelemetry
{
    /// <summary>
    /// The endpoint that the Client should send OpenTelemetry data to.
    /// </summary>
    public string? Endpoint { get; set; }
    
    /// <summary>
    /// The ServiceName that the Client will report in OpenTelemetry data.
    /// </summary>
    public string? ServiceName { get; set; }
}
