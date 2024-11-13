// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.DependencyInversion;
using Dolittle.SDK.Diagnostics.OpenTelemetry;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
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
    /// Sets how long should the aggregates be kept in memory when not in use.
    /// </summary>
    /// <param name="timeout">Duration to keep aggregates in memory when idle. -1 ms to never unload them.</param>
    /// <returns></returns>
    public IConfigurationBuilder WithAggregateIdleTimout(TimeSpan timeout);

    /// <summary>
    /// Sets the default timeout for performing an aggregate operation (potentially including hydration and committing).
    /// If no CancellationToken is provided, this timeout will be used.
    /// Not set by default
    /// </summary>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public IConfigurationBuilder WithDefaultAggregatePerformTimeout(TimeSpan timeout);
    
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

    /// <summary>
    /// Configures the <see cref="MongoDatabaseSettings"/> used.
    /// </summary>
    /// <param name="configureMongoDatabase">The callback for configuring <see cref="MongoDatabaseSettings"/>.</param>
    /// <returns>The builder for continuation.</returns>
    IConfigurationBuilder WithMongoDatabaseSettings(Action<MongoDatabaseSettings> configureMongoDatabase);
    
    /// <summary>
    /// Configures the default <see cref="GuidRepresentation"/> to use when serializing and deserializing <see cref="Guid"/> values.
    /// To disable default GuidRepresentation, set to <see cref="GuidRepresentation.Unspecified"/>.
    /// If you disable it, it will break serialization for <see cref="Guid"/> values that do not explicitly
    /// specify a representation, so use with caution.
    /// </summary>
    /// <param name="guidRepresentation">The <see cref="GuidRepresentation"/> to use.</param>
    /// <returns></returns>
    IConfigurationBuilder WithDefaultGuidRepresentation(GuidRepresentation guidRepresentation);
    
    /// <summary>
    /// Configures the <see cref="MongoClient"/> used. Please note that setting this will override the default configured tracing of MongoDB.
    /// </summary>
    /// <param name="configureMongoClient">The callback for configuring <see cref="MongoClientSettings"/>.</param>
    /// <returns>The builder for continuation.</returns>
    IConfigurationBuilder WithMongoClientSettings(Action<MongoClientSettings> configureMongoClient);
}
