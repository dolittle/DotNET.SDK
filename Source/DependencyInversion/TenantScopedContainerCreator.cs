// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.SDK.DependencyInversion;

/// <summary>
/// Represents an implementation of <see cref="ICanCreateTenantScopedContainer"/> for Microsoft's dependency injection framework.
/// </summary>
public class TenantScopedContainerCreator : ICanCreateTenantScopedContainer
{
    /// <inheritdoc />
    public bool CanCreateFrom(IServiceProvider rootProvider)
        => rootProvider is not null;

    /// <inheritdoc />
    public IServiceProvider Create(IServiceProvider rootProvider, IServiceCollection tenantScopedServices)
    {
        var containerBuilder = new ContainerBuilder();
        if (tenantScopedServices.Any(_ => _.Lifetime == ServiceLifetime.Singleton && _.ImplementationInstance == null))
        {
            throw new CannotRegisterSingletonDependenciesOnTenantScopedContainer();
        }

        containerBuilder.Populate(tenantScopedServices);
        var container = containerBuilder.Build();
        var rootScope = container.BeginLifetimeScope(builder =>
        {
            builder.RegisterInstance(new ServiceScopeFactory(rootProvider.GetRequiredService<IServiceScopeFactory>(), container)).As<IServiceScopeFactory>();
            builder.RegisterSource(new UnknownServiceOnTenantContainerRegistrationSource(rootProvider)); ;
        });

        return new AutofacServiceProvider(rootScope);
    }
}
