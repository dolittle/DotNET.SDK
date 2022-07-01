// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.DependencyInversion;
using Lamar;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.SDK.Extensions.DependencyInversion.Lamar;

/// <summary>
/// Represents an implementation of <see cref="ICanCreateTenantScopedContainer"/> using Lamar <see cref="Container"/> as the child container.
/// </summary>
public class TenantScopedContainerCreator : ICanCreateTenantScopedContainer
{
    /// <inheritdoc />
    public bool CanCreateFrom(IServiceProvider rootProvider)
        => rootProvider is IContainer;

    /// <inheritdoc />
    public IServiceProvider Create(IServiceProvider rootProvider, IServiceCollection tenantScopedServices)
    {
        var rootContainer = (rootProvider as IContainer)!;
        var tenantScopedContainer = rootContainer.GetNestedContainer() as Container;
        tenantScopedContainer!.Configure(tenantScopedServices);
        return tenantScopedContainer;
    }
}
