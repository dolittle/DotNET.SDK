// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.DependencyInversion;
using Dolittle.SDK.Microservices;
using Microsoft.Extensions.DependencyInjection;
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
    /// Gets or sets the <see cref="Func{TResult}"/> factory for <see cref="DependencyInversion.CreateTenantContainer"/>.
    /// </summary>
    public Func<IServiceProvider, CreateTenantContainer> CreateTenantContainerFactory { get; private set; }

    /// <summary>
    /// Gets or sets the <see cref="DependencyInversion.CreateTenantContainer"/>.
    /// </summary>
    public CreateTenantContainer CreateTenantContainer { get; private set; }
    

    /// <summary>
    /// Configures the <see cref="DolittleClientConfiguration"/> with the configuration values from <see cref="Configurations.Dolittle"/>.
    /// </summary>
    /// <param name="config">The <see cref="Configurations.Dolittle"/>.</param>
    /// <returns>The <see cref="DolittleClientConfiguration"/>.</returns>
    public static DolittleClientConfiguration FromConfiguration(Configurations.Dolittle config)
    {
        var result = new DolittleClientConfiguration();
        if (config.Runtime != default)
        {
            var runtime = config.Runtime;
            if (!string.IsNullOrEmpty(runtime.Host))
            {
                result.RuntimeHost = config.Runtime.Host;
            }
        
            if (runtime.Port.HasValue)
            {
                result.RuntimePort = runtime.Port.Value;
            }
        }
        if (config.PingInterval.HasValue)
        {
            result.PingInterval = TimeSpan.FromSeconds(config.PingInterval.Value);
        }
        if (!string.IsNullOrEmpty(config.HeadVersion))
        {
            result.Version = new VersionConverter().FromString(config.HeadVersion);
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
    public IConfigurationBuilder WithPingInterval(TimeSpan interval)
    {
        PingInterval = interval;
        return this;
    }

    /// <inheritdoc />
    public IConfigurationBuilder WithServiceProvider(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
        
        return this;
    }

    /// <inheritdoc />
    public IConfigurationBuilder WithRootContainerAndTenantContainerCreator<TContainer>(TContainer container, ICreateTenantContainers<TContainer> creator)
        where TContainer : class, IServiceProvider
    {
        ServiceProvider = container;
        CreateTenantContainer = services => creator.Create(container, services);
        return this;
    }
    
    /// <inheritdoc />
    public IConfigurationBuilder WithTenantContainerCreator<TContainer>(Func<TContainer, ICreateTenantContainers<TContainer>> factory)
        where TContainer : class, IServiceProvider
    {
        CreateTenantContainerFactory = provider =>
        {
            var container = ICreateTenantContainers<TContainer>.RootContainerGuard(provider);
            return services => factory(container).Create(container, services);
        };
        return this;
    }
    
    /// <inheritdoc />
    public IConfigurationBuilder WithTenantContainerCreator<TContainer>()
        where TContainer : class, IServiceProvider
    {
        CreateTenantContainerFactory = provider =>
        {
            var container = ICreateTenantContainers<TContainer>.RootContainerGuard(provider);
            return services => container.GetRequiredService<ICreateTenantContainers<TContainer>>().Create(container, services);
        };
        return this;
    }
    
    
    /// <summary>
    /// Configures the <see cref="CreateTenantContainer"/>.
    /// </summary>
    /// <param name="creator">The <see cref="DependencyInversion.CreateTenantContainer"/> delegate.</param>
    /// <returns>The builder for continuation.</returns>
    public IConfigurationBuilder WithTenantContainerCreator(CreateTenantContainer creator)
    {
        CreateTenantContainer = creator;
        return this;
    }

    /// <inheritdoc />
    public IConfigurationBuilder WithTenantServices(ConfigureTenantServices configureTenantServices)
    {
        ConfigureTenantServices = configureTenantServices;
        return this;
    }
}
