// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Dolittle.SDK.Diagnostics.OpenTelemetry;

/// <summary>
/// Configuration options for OpenTelemetry tracing and logging
/// </summary>
public record OpenTelemetrySettings
{
    /// <summary>
    /// OTLP reporting endpoint
    /// </summary>
    public string? Endpoint { get; set; }

    /// <summary>
    /// Service name as reported by OTLP
    /// </summary>
    public string ServiceName { get; set; } = AppDomain.CurrentDomain.FriendlyName;

    /// <summary>
    /// Enable logging via OTLP
    /// </summary>
    public bool Logging { get; set; } = true;

    /// <summary>
    /// Enable tracing via OTLP
    /// </summary>
    public bool Tracing { get; set; } = true;

    /// <summary>
    /// Enable metrics via OTLP - default is true
    /// </summary>
    public bool Metrics { get; set; } = true;

    /// <summary>
    /// Tracing configuration callback
    /// </summary>
    public Action<TracerProviderBuilder>? ConfigureTracing { get; set; } = null;

    /// <summary>
    /// Logging configuration callback.
    /// </summary>
    public Action<OpenTelemetryLoggerOptions>? ConfigureLogging { get; set; } = null;

    /// <summary>
    /// Metrics configuration callback.
    /// </summary>
    public Action<MeterProviderBuilder>? ConfigureMetrics { get; set; } = null;
}
