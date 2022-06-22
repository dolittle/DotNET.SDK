// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Diagnostics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Dolittle.SDK.Diagnostics.OpenTelemetry;

/// <summary>
/// Helper class to simplify setting up logs & traces via OTLP
/// </summary>
public static class OpenTelemetryConfigurationExtensions
{
    /// <summary>
    /// Configures open telemetry tracing and logging.
    /// </summary>
    /// <remarks>
    /// If <see cref="OpenTelemetrySettings.Endpoint"/> is not given the environment variable OTEL_EXPORTER_OTLP_ENDPOINT will be used.
    /// And if OTEL_EXPORTER_OTLP_ENDPOINT is not set the endpoint will default to using grpc on http://localhost:4317.
    /// </remarks>
    /// <param name="builder">The <see cref="IHostBuilder"/>.</param>
    /// <param name="getSettings">The optional <see cref="Action{T}"/> for getting <see cref="OpenTelemetrySettings"/>.</param>
    /// <returns>The <see cref="IHostBuilder"/> builder for continuation.</returns>
    public static IHostBuilder ConfigureOpenTelemetry(this IHostBuilder builder, Func<OpenTelemetrySettings> getSettings)
    {
        // Defaults:
        // Exporter Endpoint env => OTEL_EXPORTER_OTLP_ENDPOINT
        // If env not set default is: grpc on http://localhost:4317 or http on http://localhost:4318
        var settings = getSettings();
        if (settings is null)
        {
            return builder;
        }

        Uri.TryCreate(settings.Endpoint, UriKind.Absolute, out var otlpEndpoint);

#if NETCOREAPP3_0_OR_GREATER
        
        if (otlpEndpoint is not null && !otlpEndpoint.Scheme.Equals("https"))
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
        }
#endif

        var resourceBuilder = ResourceBuilder.CreateDefault().AddService(settings.ServiceName);
        if (settings.Logging)
        {
            builder.AddOpenTelemetryLogging(resourceBuilder, otlpEndpoint, settings.ConfigureLogging);
        }

        if (settings.Tracing)
        {
            builder.AddOpenTelemetryTracing(resourceBuilder, otlpEndpoint, settings.ConfigureTracing);
        }

        return builder;
    }

    static void AddOpenTelemetryLogging(this IHostBuilder builder, ResourceBuilder resourceBuilder, Uri? otlpEndpoint, Action<OpenTelemetryLoggerOptions>? configure)
    {
        builder.ConfigureLogging(loggingBuilder =>
        {
            loggingBuilder
                // Span & Trace ID's are published by OTLP without adding them to scope
                .Configure(options => options.ActivityTrackingOptions = ActivityTrackingOptions.None)
                .AddOpenTelemetry(options =>
                {
                    options.IncludeScopes = true;
                    options.IncludeFormattedMessage = true;
                    options.SetResourceBuilder(resourceBuilder)
                        .AddOtlpExporter(ConfigureOtlpExporter(otlpEndpoint));
                    configure?.Invoke(options);
                });
        });
    }

    static void AddOpenTelemetryTracing(this IHostBuilder builder, ResourceBuilder resourceBuilder, Uri? otlpEndpoint, Action<TracerProviderBuilder>? configure)
    {
        builder.ConfigureServices(services =>
            services.AddOpenTelemetryTracing(builder =>
            {
                builder.SetResourceBuilder(resourceBuilder)
                    .AddDolittleInstrumentation()
                    .AddOtlpExporter(ConfigureOtlpExporter(otlpEndpoint));
                configure?.Invoke(builder);
            }));
    }

    static Action<OtlpExporterOptions> ConfigureOtlpExporter(Uri? otlpEndpoint)
        => otlpEndpoint is not null
            ? _ => _.Endpoint = otlpEndpoint
            : _ => { };
}
