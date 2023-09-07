// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.SDK.DependencyInversion;
using Dolittle.SDK.Diagnostics.OpenTelemetry;
using Dolittle.SDK.Microservices;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Core.Extensions.DiagnosticSources;
using Newtonsoft.Json;
using Version = Dolittle.SDK.Microservices.Version;

namespace Dolittle.SDK.Builders;

/// <summary>
/// Represents the <see cref="IDolittleClient"/> configuration.
/// </summary>
public class DolittleClientConfiguration : IConfigurationBuilder
{
    /// <summary>
    /// Gets the <see cref="Version"/> of the Head.
    /// </summary>
    public Version Version { get; private set; } = Version.NotSet;

    /// <summary>
    /// Gets the Runtime host.
    /// </summary>
    public string RuntimeHost { get; private set; } = "localhost";

    /// <summary>
    /// Gets the Runtime port.
    /// </summary>
    public ushort RuntimePort { get; private set; } = 50053;

    /// <summary>
    /// Gets the ping-interval <see cref="TimeSpan"/>.
    /// </summary>
    public TimeSpan PingInterval { get; private set; } = TimeSpan.FromSeconds(5);

    /// <summary>
    /// How long should the aggregates be kept in memory when not in use.
    /// </summary>
    public TimeSpan AggregateIdleTimeout { get; private set; } = TimeSpan.FromSeconds(30);
    
    /// <summary>
    /// The default timeout for performing an aggregate operation (potentially including hydration & commits).
    /// If no CancellationToken is provided, this timeout will be used.
    /// </summary>
    public TimeSpan? DefaultAggregatePerformTimeout { get; private set; }

    /// <summary>
    /// Gets the event serializer provider.
    /// </summary>
    public Func<JsonSerializerSettings> EventSerializerProvider { get; private set; } = () => new JsonSerializerSettings();

    /// <summary>
    /// Gets the OpenTelemetry settings provider.
    /// </summary>
    public Func<OpenTelemetrySettings> OpenTelemetrySettingsProvider { get; private set; } = () => new OpenTelemetrySettings();

    /// <summary>
    /// Gets the <see cref="ILoggerFactory"/>.
    /// </summary>
    public ILoggerFactory? LoggerFactory { get; private set; }

    /// <summary>
    /// Gets the <see cref="IServiceProvider"/>.
    /// </summary>
    public IServiceProvider? ServiceProvider { get; private set; }

    /// <summary>
    /// Gets the <see cref="ConfigureTenantServices"/> callback.
    /// </summary>
    public ConfigureTenantServices? ConfigureTenantServices { get; private set; }

    /// <summary>
    /// Gets the <see cref="CreateTenantServiceProvider"/> factory.
    /// </summary>
    public CreateTenantServiceProvider? TenantServiceProviderFactory { get; private set; }

    /// <summary>
    /// Gets the callback for configuring the <see cref="MongoDatabaseSettings"/>.
    /// </summary>
    public Action<MongoDatabaseSettings>? ConfigureMongoDatabaseSettings { get; private set; }

    /// <summary>
    /// Gets the callback for configuring the <see cref="MongoClientSettings"/>.
    /// </summary>
    public Action<MongoClientSettings> ConfigureMongoClientSettings { get; private set; } = settings =>
        settings.ClusterConfigurator = cb => cb.Subscribe(new DiagnosticsActivityEventSubscriber());

    /// <summary>
    /// Configures the <see cref="DolittleClientConfiguration"/> with the configuration values from <see cref="Configurations.Dolittle"/>.
    /// </summary>
    /// <param name="config">The <see cref="Configurations.Dolittle"/>.</param>
    /// <returns>The <see cref="DolittleClientConfiguration"/>.</returns>
    public static DolittleClientConfiguration FromConfiguration(Configurations.Dolittle config)
    {
        var result = new DolittleClientConfiguration();

        if (!string.IsNullOrEmpty(config.Runtime.Host))
        {
            result.RuntimeHost = config.Runtime.Host!;
        }

        if (config.Runtime.Port.HasValue)
        {
            result.RuntimePort = config.Runtime.Port.Value;
        }

        if (config.PingInterval.HasValue)
        {
            result.PingInterval = TimeSpan.FromSeconds(config.PingInterval.Value);
        }

        if (config.AggregateIdleTimout.HasValue)
        {
            result.AggregateIdleTimeout =
                config.AggregateIdleTimout.Value == -1
                    ? Timeout.InfiniteTimeSpan
                    : TimeSpan.FromSeconds(config.AggregateIdleTimout.Value);
        }

        if (!string.IsNullOrEmpty(config.HeadVersion))
        {
            result.Version = new VersionConverter().FromString(config.HeadVersion);
        }

        if (config.Otlp is not null)
        {
            result.OpenTelemetrySettingsProvider = () => new OpenTelemetrySettings
            {
                Endpoint = config.Otlp.Endpoint,
                ServiceName = config.Otlp.ServiceName,
            };
        }

        return result;
    }

    /// <inheritdoc />
    public IConfigurationBuilder WithVersion(Version version)
    {
        Version = version;
        return this;
    }

    /// <inheritdoc />
    public IConfigurationBuilder WithRuntimeOn(string host, ushort port)
    {
        RuntimeHost = host;
        RuntimePort = port;
        return this;
    }

    /// <inheritdoc />
    public IConfigurationBuilder WithLogging(ILoggerFactory factory)
    {
        LoggerFactory = factory;
        return this;
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
    public IConfigurationBuilder WithOpenTelemetrySettings(Action<OpenTelemetrySettings> openTelemetrySettingsBuilder)
    {
        var provider = OpenTelemetrySettingsProvider;
        OpenTelemetrySettingsProvider = () =>
        {
            var settings = provider();
            openTelemetrySettingsBuilder.Invoke(settings);
            return settings;
        };

        return this;
    }

    /// <inheritdoc />
    public IConfigurationBuilder WithPingInterval(TimeSpan interval)
    {
        PingInterval = interval;
        return this;
    }

    /// <inheritdoc />
    public IConfigurationBuilder WithAggregateIdleTimout(TimeSpan timeout)
    {
        AggregateIdleTimeout = timeout;
        return this;
    }
    
    /// <inheritdoc />
    public IConfigurationBuilder WithDefaultAggregatePerformTimeout(TimeSpan timeout)
    {
        DefaultAggregatePerformTimeout = timeout;
        return this;
    }

    /// <inheritdoc />
    public IConfigurationBuilder WithServiceProvider(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;

        return this;
    }

    /// <inheritdoc />
    public IConfigurationBuilder WithTenantServices(ConfigureTenantServices configureTenantServices)
    {
        ConfigureTenantServices = configureTenantServices;
        return this;
    }

    /// <inheritdoc />
    public IConfigurationBuilder WithTenantServiceProviderFactory(CreateTenantServiceProvider factory)
    {
        TenantServiceProviderFactory = factory;
        return this;
    }

    /// <inheritdoc />
    public IConfigurationBuilder WithMongoDatabaseSettings(Action<MongoDatabaseSettings> configureMongoDatabase)
    {
        ConfigureMongoDatabaseSettings = configureMongoDatabase;
        return this;
    }

    /// <inheritdoc />
    public IConfigurationBuilder WithMongoClientSettings(Action<MongoClientSettings> configureMongoClient)
    {
        ConfigureMongoClientSettings = configureMongoClient;
        return this;
    }
}
