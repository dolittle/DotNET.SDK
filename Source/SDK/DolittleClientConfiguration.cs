// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.SDK.DependencyInversion;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Dolittle.SDK
{
    /// <summary>
    /// Represents the <see cref="IDolittleClient"/> configuration.
    /// </summary>
    public class DolittleClientConfiguration
    {
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
        /// Gets the event serializer provider.
        /// </summary>
        public Func<JsonSerializerSettings> EventSerializerProvider { get; private set; } = () => new JsonSerializerSettings();

        /// <summary>
        /// Gets the <see cref="ILoggerFactory"/>.
        /// </summary>
        public ILoggerFactory LoggerFactory { get; private set; } = Microsoft.Extensions.Logging.LoggerFactory.Create(_ =>
        {
            _.SetMinimumLevel(LogLevel.Information);
            _.AddConsole();
        });

        /// <summary>
        /// Gets the <see cref="IServiceProvider"/>.
        /// </summary>
        public IServiceProvider ServiceProvider { get; private set; } = new DefaultServiceProviderFactory().CreateServiceProvider(new ServiceCollection());

        /// <summary>
        /// Gets the <see cref="ConfigureTenantServices"/> callback.
        /// </summary>
        public ConfigureTenantServices ConfigureTenantServices { get; private set; } = (_, __) => { };

        /// <summary>
        /// Connect to a specific host and port for the Dolittle runtime.
        /// </summary>
        /// <param name="host">The host name to connect to.</param>
        /// <param name="port">The port to connect to.</param>
        /// <returns>the client configuration builder for continuation.</returns>
        /// <remarks>If not specified, host 'localhost' and port 50053 will be used.</remarks>
        public DolittleClientConfiguration WithRuntimeOn(string host, ushort port)
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
        public DolittleClientConfiguration WithLogging(ILoggerFactory factory)
        {
            LoggerFactory = factory;
            return this;
        }

        /// <summary>
        /// Sets a callback that configures the <see cref="JsonSerializerSettings"/> for serializing events.
        /// </summary>
        /// <param name="jsonSerializerSettingsBuilder"><see cref="Action{T}"/> that gets called with <see cref="JsonSerializerSettings"/> to modify settings.</param>
        /// <returns>the client configuration builder for continuation.</returns>
        public DolittleClientConfiguration WithEventSerializerSettings(Action<JsonSerializerSettings> jsonSerializerSettingsBuilder)
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
        public DolittleClientConfiguration WithPingInterval(TimeSpan interval)
        {
            PingInterval = interval;
            return this;
        }

        /// <summary>
        /// Configures the root <see cref="IServiceProvider"/> for the <see cref="IDolittleClient"/>.
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/>.</param>
        /// <returns>The client for continuation.</returns>
        public DolittleClientConfiguration WithServiceProvider(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            return this;
        }

        /// <summary>
        /// Configures a <see cref="ConfigureTenantServices"/> callback for configuring the tenant specific IoC containers.
        /// </summary>
        /// <param name="configureTenantServices">The <see cref="ConfigureTenantServices"/> callback.</param>
        /// <returns>The client for continuation.</returns>
        public DolittleClientConfiguration WithTenantServices(ConfigureTenantServices configureTenantServices)
        {
            ConfigureTenantServices = configureTenantServices;
            return this;
        }
    }
}