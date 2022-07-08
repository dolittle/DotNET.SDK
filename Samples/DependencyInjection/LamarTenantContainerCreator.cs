// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.DependencyInversion;
using Dolittle.SDK.Tenancy;
using Lamar;
using Microsoft.Extensions.DependencyInjection;

public class LamarTenantContainerCreator : TenantContainerCreator<Container>
{
    protected override IServiceProvider CreateFromContainer(Container container, TenantId tenant, IServiceCollection tenantServices)
    {
        var childContainer = (Container)container.GetNestedContainer();
        childContainer.Configure(tenantServices);
        return childContainer;
    }
}
