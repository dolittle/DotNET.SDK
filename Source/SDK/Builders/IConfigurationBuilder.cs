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
/// Defines a builder for the Dolittle configuration.
/// </summary>
public interface IConfigurationBuilder
{
    /// <summary>
    /// Sets the <see cref="Version"/> of the Head.
    /// </summary>
    /// <param name="version">The <see cref="Version"/>.</param>
    /// <returns></returns>
    public IConfigurationBuilder WithVersion(Version version);

    /// <summary>
    /// Connect to a specific host and port for the Dolittle runtime.
    /// </summary>
    /// <param name="host">The host name to connect to.</param>
    /// <param name="port">The port to connect to.</param>
    /// <returns>the client configuration builder for continuation.</returns>
    /// <remarks>If not specified, host 'localhost' and port 50053 will be used.</remarks>
    public IConfigurationBuilder WithRuntimeOn(string host, ushort port);
    /// <summary>
    /// Sets the <see cref="ILoggerFactory"/> to use for creating instances of <see cref="ILogger"/> for the client configuration.
    /// </summary>
    /// <param name="factory">The given <see cref="ILoggerFactory"/>.</param>
    /// <returns>the client configuration builder for continuation.</returns>
    /// <remarks>If not used, a factory with 'Trace' level logging will be used.</remarks>
    public IConfigurationBuilder WithLogging(ILoggerFactory factory);

    /// <summary>
    /// Sets a callback that configures the <see cref="JsonSerializerSettings"/> for serializing events.
    /// </summary>
    /// <param name="jsonSerializerSettingsBuilder"><see cref="Action{T}"/> that gets called with <see cref="JsonSerializerSettings"/> to modify settings.</param>
    /// <returns>the client configuration builder for continuation.</returns>
    public IConfigurationBuilder WithEventSerializerSettings(Action<JsonSerializerSettings> jsonSerializerSettingsBuilder);

    /// <summary>
    /// Sets a callback that configures the <see cref="JsonSerializerSettings"/> for serializing events.
    /// </summary>
    /// <param name="openTelemetrySettingsBuilder"><see cref="Action{T}"/> that gets called with <see cref="OpenTelemetrySettings"/> to modify settings.</param>
    /// <returns>the client configuration builder for continuation.</returns>
    public IConfigurationBuilder WithOpenTelemetrySettings(Action<OpenTelemetrySettings> openTelemetrySettingsBuilder);

    /// <summary>
    /// Sets the ping interval for communicating with the microservice.
    /// </summary>
    /// <param name="interval">The ping interval.</param>
    /// <returns>the client configuration builder for continuation.</returns>
    public IConfigurationBuilder WithPingInterval(TimeSpan interval);

    /// <summary>
    /// Configures the root <see cref="IServiceProvider"/> for the <see cref="IDolittleClient"/>.
    /// </summary>
    /// <param name="serviceProvider">The <see cref="IDolittleClient"/>.</param>
    /// <returns>The client for continuation.</returns>
    IConfigurationBuilder WithServiceProvider(IServiceProvider serviceProvider);

    /// <summary>
    /// Configures a <see cref="ConfigureTenantServices"/> callback for configuring the tenant specific IoC containers.
    /// </summary>
    /// <param name="configureTenantServices">The <see cref="ConfigureTenantServices"/> callback.</param>
    /// <returns>The client for continuation.</returns>
    IConfigurationBuilder WithTenantServices(ConfigureTenantServices configureTenantServices);

    /// <summary>
    /// Configures a <see cref="CreateTenantServiceProvider"/> factory to use when creating tenant specific IoC containers.
    /// </summary>
    /// <param name="factory">The <see cref="CreateTenantServiceProvider"/> factory.</param>
    /// <returns>The client for continuation.</returns>
    IConfigurationBuilder WithTenantServiceProviderFactory(CreateTenantServiceProvider factory);
}
