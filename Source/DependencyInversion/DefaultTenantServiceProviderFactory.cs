// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Autofac;
using Autofac.Core;
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
        containerBuilder.Populate(services);
        containerBuilder.RegisterSource(new UnknownServiceOnTenantContainerRegistrationSource(provider, Enumerable.Empty<IComponentRegistration>(), true));
        containerBuilder.Register(_ => new ServiceScopeFactory(provider.GetRequiredService<IServiceScopeFactory>(), _.Resolve<ILifetimeScope>())).As<IServiceScopeFactory>().SingleInstance();
        var container = containerBuilder.Build();
        return new AutofacServiceProvider(container);
    };
}
