// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Tenancy;

public abstract class Service : IDisposable
{
    protected Service()
    {
        Console.WriteLine($"Creating {GetType().Name}");   
    }
    public void Dispose() => Console.WriteLine($"Disposing {GetType().Name}");
}
public abstract class TenantScopedService : IDisposable
{
    readonly TenantId _tenant;

    protected TenantScopedService(TenantId tenant)
    {
        Console.WriteLine($"Creating {GetType().Name} for tenant {tenant}");
        _tenant = tenant;
    }

    public void Dispose() => Console.WriteLine($"Disposing {GetType().Name} for tenant {_tenant}");
}
