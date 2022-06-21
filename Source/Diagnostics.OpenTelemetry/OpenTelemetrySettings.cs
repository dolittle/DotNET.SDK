// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using OpenTelemetry.Trace;

namespace Dolittle.SDK.Diagnostics.OpenTelemetry;

public record OpenTelemetrySettings
{
    /// <summary>
    /// OTLP reporting endpoint
    /// </summary>
    public string? Endpoint { get; set; }

    /// <summary>
    /// Service name as reported by OTLP
    /// </summary>
    public string ServiceName { get; set; } = "dolittle-app";

    /// <summary>
    /// Enable logging via OTLP
    /// </summary>
    public bool Logging { get; set; } = true;

    /// <summary>
    /// Enable tracing via OTLP
    /// </summary>
    public bool Tracing { get; set; } = true;

    /// <summary>
    /// Tracing configuration callback
    /// </summary>
    public Action<TracerProviderBuilder>? ConfigureTracing { get; set; } = null;
}
