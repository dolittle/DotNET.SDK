// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Dolittle.SDK.DependencyInversion;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.SDK.Extensions.DependencyInversion.Autofac;

/// <summary>
/// Represents an implementation of <see cref="ICanCreateTenantScopedContainer"/> using <see cref="AutofacServiceProvider"/> as the child container.
/// </summary>
public class TenantScopedContainerCreator : ICanCreateTenantScopedContainer
{
    /// <inheritdoc />
    public bool CanCreateFrom(IServiceProvider rootProvider)
        => rootProvider is AutofacServiceProvider;

    /// <inheritdoc />
    public IServiceProvider Create(IServiceProvider rootProvider, IServiceCollection tenantScopedServices)
        => new AutofacServiceProvider(GetLifetimeScope(rootProvider).BeginLifetimeScope(_ => _.Populate(tenantScopedServices)));

    static ILifetimeScope GetLifetimeScope(IServiceProvider serviceProvider)
        => serviceProvider switch
        {
            AutofacServiceProvider provider => provider.LifetimeScope,
            _ => throw new ArgumentException($"Service provider needs to be of type {typeof(AutofacServiceProvider)}", nameof(serviceProvider))
        };
}
