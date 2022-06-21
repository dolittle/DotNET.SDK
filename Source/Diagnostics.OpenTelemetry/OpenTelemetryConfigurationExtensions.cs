// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Diagnostics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
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
    /// 
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="settings"></param>
    /// <returns></returns>
    public static IHostBuilder ConfigureOpenTelemetry(this IHostBuilder builder, OpenTelemetrySettings settings)
    {
        if (settings?.Endpoint is null)
        {
            return builder;
        }

        if (!Uri.TryCreate(settings.Endpoint, UriKind.Absolute, out var otlpEndpoint))
        {
            return builder;
        }

#if NETCOREAPP3_0_OR_GREATER
        if (otlpEndpoint.Scheme.Equals("http"))
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
        }
#endif

        var resourceBuilder = ResourceBuilder.CreateDefault().AddService(settings.ServiceName);
        if (settings.Logging)
        {
            builder.AddOpenTelemetryLogging(resourceBuilder, otlpEndpoint);
        }

        if (settings.Tracing)
        {
            builder.AddOpenTelemetryTracing(resourceBuilder, otlpEndpoint);
        }

        return builder;
    }

    static void AddOpenTelemetryLogging(this IHostBuilder builder, ResourceBuilder resourceBuilder, Uri otlpEndpoint)
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
                        .AddOtlpExporter(otlpOptions => otlpOptions.Endpoint = otlpEndpoint);
                });
        });
    }

    static void AddOpenTelemetryTracing(this IHostBuilder builder, ResourceBuilder resourceBuilder, Uri otlpEndpoint, Action<TracerProviderBuilder>? configure = null)
    {
        builder.ConfigureServices(services =>
            services.AddOpenTelemetryTracing(builder =>
            {
                builder.SetResourceBuilder(resourceBuilder)
                    .AddDolittleInstrumentation()
                    .AddOtlpExporter(options => { options.Endpoint = otlpEndpoint; });
                configure?.Invoke(builder);
            }));
    }
}
