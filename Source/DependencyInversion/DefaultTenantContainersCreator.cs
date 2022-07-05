// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.SDK.DependencyInversion;

/// <summary>
/// Represents an implementation of <see cref="ICreateTenantContainers{TContainer}"/> for Microsoft's dependency injection framework.
/// </summary>
public class DefaultTenantContainersCreator : ICreateTenantContainers<IServiceProvider>
{
    /// <inheritdoc />
    public IServiceProvider Create(IServiceProvider rootProvider, IServiceCollection tenantScopedServices)
    {
        if (rootProvider is AutofacServiceProvider provider)
        {
            return new AutofacServiceProvider(provider.LifetimeScope.BeginLifetimeScope(_ => _.Populate(tenantScopedServices)));
        }
        var containerBuilder = new ContainerBuilder();
        containerBuilder.Populate(tenantScopedServices);
        containerBuilder.RegisterSource(new UnknownServiceOnTenantContainerRegistrationSource(rootProvider, Enumerable.Empty<IComponentRegistration>()));
        containerBuilder.Register(_ => new ServiceScopeFactory(rootProvider.GetRequiredService<IServiceScopeFactory>(), _.Resolve<ILifetimeScope>())).As<IServiceScopeFactory>().SingleInstance();
        var container = containerBuilder.Build();
        return new AutofacServiceProvider(container);
    }
}
