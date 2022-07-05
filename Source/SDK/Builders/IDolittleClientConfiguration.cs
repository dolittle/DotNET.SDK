// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.DependencyInversion;
using Dolittle.SDK.Diagnostics.OpenTelemetry;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Version = Dolittle.SDK.Microservices.Version;

namespace Dolittle.SDK.Builders;

/// <summary>
/// Defines the dolittle client configuration.
/// </summary>
public interface IDolittleClientConfiguration
{
    /// <summary>
    /// Gets or sets the <see cref="Version"/> of the Head.
    /// </summary>
    Version Version { get; }

    /// <summary>
    /// Gets or sets the Runtime host.
    /// </summary>
    string RuntimeHost { get; }

    /// <summary>
    /// Gets or sets theRuntime port.
    /// </summary>
    ushort RuntimePort { get; }

    /// <summary>
    /// Gets or sets the ping-interval <see cref="TimeSpan"/>.
    /// </summary>
    TimeSpan PingInterval { get; }

    /// <summary>
    /// Gets the event serializer provider.
    /// </summary>
    Func<JsonSerializerSettings> EventSerializerProvider { get; }

    /// <summary>
    /// Gets the event serializer provider
    /// </summary>
    Func<OpenTelemetrySettings> OpenTelemetrySettingsProvider { get; }

    /// <summary>
    /// Gets the<see cref="ILoggerFactory"/>.
    /// </summary>
    ILoggerFactory LoggerFactory { get; }

    /// <summary>
    /// Gets or sets the<see cref="IServiceProvider"/>.
    /// </summary>
    IServiceProvider ServiceProvider { get; }
}
