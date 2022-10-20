// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.SDK.DependencyInversion;

/// <summary>
/// Represents a default implementation of <see cref="CreateTenantServiceProvider"/>.
/// </summary>
public static class DefaultTenantServiceProviderFactory
{
    /// <summary>
    /// An instance of the default implementation of <see cref="CreateTenantServiceProvider"/> that uses Autofac.
    /// </summary>
    public static CreateTenantServiceProvider Instance { get; } = (provider, tenant, services) =>
    {
        if (provider is AutofacServiceProvider autofacProvider)
        {
            return new AutofacServiceProvider(autofacProvider.LifetimeScope.BeginLifetimeScope(_ => _.Populate(services)));
        }

        var containerBuilder = new ContainerBuilder();
        var registrationSource = new UnknownServiceOnTenantContainerRegistrationSource(provider, true);
        containerBuilder.Populate(services);
        containerBuilder.RegisterSource(registrationSource);
        containerBuilder.Register(_ => new ServiceScopeFactory(provider.GetRequiredService<IServiceScopeFactory>(), _.Resolve<ILifetimeScope>())).As<IServiceScopeFactory>().SingleInstance();
        var container = containerBuilder.Build();
        registrationSource.Registrations = container.ComponentRegistry.Registrations;
        return new AutofacServiceProvider(container);
    };
}
