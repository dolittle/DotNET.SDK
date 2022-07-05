// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.DependencyInversion;
using Lamar;
using Microsoft.Extensions.DependencyInjection;

public class LamarTenantContainerCreator : ICreateTenantContainers<Container>
{
    public IServiceProvider Create(Container rootContainer, IServiceCollection tenantScopedServices)
    {
        var childContainer = rootContainer.GetNestedContainer() as Container;
        childContainer.Configure(tenantScopedServices);
        return childContainer;
    }
}
