// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
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
        /// Initializes a new instance of the <see cref="DolittleClientConfiguration"/> class.
        /// </summary>
        /// <param name="runtimeHost">The host of the Runtime.</param>
        /// <param name="runtimePort">The port of the Runtime.</param>
        /// <param name="pingInterval">The <see cref="TimeSpan"/> of the ping-interval.</param>
        /// <param name="jsonSerializerSettingsBuilder">The <see cref="Action{T}"/> callback for configuring <see cref="JsonSerializerSettings"/> provider.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/>.</param>
        /// <param name="cancellation">The <see cref="CancellationToken"/>.</param>
        public DolittleClientConfiguration(
            string runtimeHost,
            ushort runtimePort,
            TimeSpan pingInterval,
            Action<JsonSerializerSettings> jsonSerializerSettingsBuilder,
            ILoggerFactory loggerFactory,
            CancellationToken cancellation)
        {
            RuntimeHost = runtimeHost;
            RuntimePort = runtimePort;
            PingInterval = pingInterval;
            EventSerializerProvider = () =>
            {
                var settings = new JsonSerializerSettings();
                jsonSerializerSettingsBuilder?.Invoke(settings);
                return settings;
            };
            LoggerFactory = loggerFactory;
            Cancellation = cancellation;
        }

        /// <summary>
        /// Gets the Runtime host.
        /// </summary>
        public string RuntimeHost { get; }

        /// <summary>
        /// Gets the Runtime port.
        /// </summary>
        public ushort RuntimePort { get; }

        /// <summary>
        /// Gets the ping-interval <see cref="TimeSpan"/>
        /// </summary>
        public TimeSpan PingInterval { get; }

        /// <summary>
        /// Gets the event serializer provider.
        /// </summary>
        public Func<JsonSerializerSettings> EventSerializerProvider { get; }

        /// <summary>
        /// Gets the <see cref="ILoggerFactory"/>.
        /// </summary>
        public ILoggerFactory LoggerFactory { get; }

        /// <summary>
        /// Gets the <see cref="CancellationToken"/>.
        /// </summary>
        public CancellationToken Cancellation { get; }

        /// <summary>
        /// Gets a <see cref="DolittleClientConfigurationBuilder"/>.
        /// </summary>
        public static DolittleClientConfigurationBuilder Builder
            => new DolittleClientConfigurationBuilder();
    }
}