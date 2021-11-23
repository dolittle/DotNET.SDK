// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Dolittle.SDK
{
    /// <summary>
    /// Represents the builder for <see cref="DolittleClientConfiguration"/>.
    /// </summary>
    public class DolittleClientConfigurationBuilder
    {
        string _host = "localhost";
        ushort _port = 50053;
        TimeSpan _pingInterval = TimeSpan.FromSeconds(5);
        Action<JsonSerializerSettings> _jsonSerializerSettingsBuilder;

        ILoggerFactory _loggerFactory = LoggerFactory.Create(_ =>
        {
            _.SetMinimumLevel(LogLevel.Information);
            _.AddConsole();
        });

        CancellationToken _cancellation = CancellationToken.None;

        /// <summary>
        /// Connect to a specific host and port for the Dolittle runtime.
        /// </summary>
        /// <param name="host">The host name to connect to.</param>
        /// <param name="port">The port to connect to.</param>
        /// <returns>the client configuration builder for continuation.</returns>
        /// <remarks>If not specified, host 'localhost' and port 50053 will be used.</remarks>
        public DolittleClientConfigurationBuilder WithRuntimeOn(string host, ushort port)
        {
            _host = host;
            _port = port;
            return this;
        }

        /// <summary>
        /// Sets the cancellation token for cancelling pending operations on the Runtime.
        /// </summary>
        /// <param name="cancellation">The cancellation token for cancelling pending operations on the Runtime.</param>
        /// <returns>the client configuration builder for continuation.</returns>
        public DolittleClientConfigurationBuilder WithCancellation(CancellationToken cancellation)
        {
            _cancellation = cancellation;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="ILoggerFactory"/> to use for creating instances of <see cref="ILogger"/> for the client configuration.
        /// </summary>
        /// <param name="factory">The given <see cref="ILoggerFactory"/>.</param>
        /// <returns>the client configuration builder for continuation.</returns>
        /// <remarks>If not used, a factory with 'Trace' level logging will be used.</remarks>
        public DolittleClientConfigurationBuilder WithLogging(ILoggerFactory factory)
        {
            _loggerFactory = factory;
            return this;
        }

        /// <summary>
        /// Sets a callback that configures the <see cref="JsonSerializerSettings"/> for serializing events.
        /// </summary>
        /// <param name="jsonSerializerSettingsBuilder"><see cref="Action{T}"/> that gets called with <see cref="JsonSerializerSettings"/> to modify settings.</param>
        /// <returns>the client configuration builder for continuation.</returns>
        public DolittleClientConfigurationBuilder WithEventSerializerSettings(Action<JsonSerializerSettings> jsonSerializerSettingsBuilder)
        {
            _jsonSerializerSettingsBuilder = jsonSerializerSettingsBuilder;
            return this;
        }

        /// <summary>
        /// Sets the ping interval for communicating with the microservice.
        /// </summary>
        /// <param name="interval">The ping interval.</param>
        /// <returns>the client configuration builder for continuation.</returns>
        public DolittleClientConfigurationBuilder WithPingInterval(TimeSpan interval)
        {
            _pingInterval = interval;
            return this;
        }

        /// <summary>
        /// Builds the <see cref="DolittleClientConfiguration"/>.
        /// </summary>
        /// <returns>The built <see cref="DolittleClientConfiguration"/>.</returns>
        public DolittleClientConfiguration Build()
            => new DolittleClientConfiguration(
                _host,
                _port,
                _pingInterval,
                _jsonSerializerSettingsBuilder,
                _loggerFactory,
                _cancellation);
    }
}