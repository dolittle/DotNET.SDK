// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.DependencyInversion;
using Dolittle.SDK.Microservices;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Version = Dolittle.SDK.Microservices.Version;
namespace Dolittle.SDK.Builders;

/// <summary>
/// Represents the <see cref="IDolittleClient"/> configuration.
/// </summary>
public class DolittleClientConfiguration : IConfigurationBuilder
{
    /// <summary>
    /// Gets or sets the <see cref="Version"/> of the Head.
    /// </summary>
    public Version Version { get; private set; } = Version.NotSet;

    /// <summary>
    /// Gets or sets the Runtime host.
    /// </summary>
    public string RuntimeHost { get; private set; } = "localhost";

    /// <summary>
    /// Gets or sets theRuntime port.
    /// </summary>
    public ushort RuntimePort { get; private set; } = 50053;

    /// <summary>
    /// Gets or sets the ping-interval <see cref="TimeSpan"/>.
    /// </summary>
    public TimeSpan PingInterval { get; private set; } = TimeSpan.FromSeconds(5);

    /// <summary>
    /// Gets or sets the event serializer provider.
    /// </summary>
    public Func<JsonSerializerSettings> EventSerializerProvider { get; private set; } = () => new JsonSerializerSettings();

    /// <summary>
    /// Gets or sets the<see cref="ILoggerFactory"/>.
    /// </summary>
    public ILoggerFactory LoggerFactory { get; private set; } = Microsoft.Extensions.Logging.LoggerFactory.Create(_ =>
    {
        _.SetMinimumLevel(LogLevel.Information);
        _.AddConsole();
    });

    /// <summary>
    /// Gets or sets the<see cref="IServiceProvider"/>.
    /// </summary>
    public IServiceProvider ServiceProvider { get; private set; }

    /// <summary>
    /// Gets or sets the<see cref="ConfigureTenantServices"/> callback.
    /// </summary>
    public ConfigureTenantServices ConfigureTenantServices { get; private set; }

    /// <summary>
    /// Configures the <see cref="DolittleClientConfiguration"/> with the configuration values from <see cref="Configurations.Dolittle"/>.
    /// </summary>
    /// <param name="config">The <see cref="Configurations.Dolittle"/>.</param>
    /// <returns>The <see cref="DolittleClientConfiguration"/>.</returns>
    public static DolittleClientConfiguration FromConfiguration(Configurations.Dolittle config)
    {
        var result = new DolittleClientConfiguration();
        if (!string.IsNullOrEmpty(config?.Runtime?.Host))
        {
            result.RuntimeHost = config.Runtime.Host;
        }
        if (config?.Runtime?.Port != default)
        {
            result.RuntimePort = config.Runtime.Port;
        }
        if (config?.PingInterval != default)
        {
            result.PingInterval = TimeSpan.FromSeconds(config.PingInterval);
        }
        if (!string.IsNullOrEmpty(config?.HeadVersion))
        {
            result.Version = new VersionConverter().FromString(config.HeadVersion);
        }
        return result;
    }
    
    /// <summary>
    /// Sets the <see cref="Version"/> of the Head.
    /// </summary>
    /// <param name="version">The <see cref="Version"/>.</param>
    /// <returns></returns>
    public IConfigurationBuilder WithVersion(Version version)
    {
        Version = version;
        return this;
    }
    
    /// <summary>
    /// Connect to a specific host and port for the Dolittle runtime.
    /// </summary>
    /// <param name="host">The host name to connect to.</param>
    /// <param name="port">The port to connect to.</param>
    /// <returns>the client configuration builder for continuation.</returns>
    /// <remarks>If not specified, host 'localhost' and port 50053 will be used.</remarks>
    public IConfigurationBuilder WithRuntimeOn(string host, ushort port)
    {
        RuntimeHost = host;
        RuntimePort = port;
        return this;
    }

    /// <summary>
    /// Sets the <see cref="ILoggerFactory"/> to use for creating instances of <see cref="ILogger"/> for the client configuration.
    /// </summary>
    /// <param name="factory">The given <see cref="ILoggerFactory"/>.</param>
    /// <returns>the client configuration builder for continuation.</returns>
    /// <remarks>If not used, a factory with 'Trace' level logging will be used.</remarks>
    public IConfigurationBuilder WithLogging(ILoggerFactory factory)
    {
        LoggerFactory = factory;
        return this;
    }

    /// <summary>
    /// Sets a callback that configures the <see cref="JsonSerializerSettings"/> for serializing events.
    /// </summary>
    /// <param name="jsonSerializerSettingsBuilder"><see cref="Action{T}"/> that gets called with <see cref="JsonSerializerSettings"/> to modify settings.</param>
    /// <returns>the client configuration builder for continuation.</returns>
    public IConfigurationBuilder WithEventSerializerSettings(Action<JsonSerializerSettings> jsonSerializerSettingsBuilder)
    {
        EventSerializerProvider = () =>
        {
            var settings = new JsonSerializerSettings();
            jsonSerializerSettingsBuilder?.Invoke(settings);
            return settings;
        };
        return this;
    }

    /// <summary>
    /// Sets the ping interval for communicating with the microservice.
    /// </summary>
    /// <param name="interval">The ping interval.</param>
    /// <returns>the client configuration builder for continuation.</returns>
    public IConfigurationBuilder WithPingInterval(TimeSpan interval)
    {
        PingInterval = interval;
        return this;
    }

    /// <summary>
    /// Configures the root <see cref="IServiceProvider"/> for the <see cref="IDolittleClient"/>.
    /// </summary>
    /// <param name="serviceProvider">The <see cref="IServiceProvider"/>.</param>
    /// <returns>The client for continuation.</returns>
    public IConfigurationBuilder WithServiceProvider(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
        return this;
    }

    /// <summary>
    /// Configures a <see cref="ConfigureTenantServices"/> callback for configuring the tenant specific IoC containers.
    /// </summary>
    /// <param name="configureTenantServices">The <see cref="ConfigureTenantServices"/> callback.</param>
    /// <returns>The client for continuation.</returns>
    public IConfigurationBuilder WithTenantServices(ConfigureTenantServices configureTenantServices)
    {
        ConfigureTenantServices = configureTenantServices;
        return this;
    }
}
