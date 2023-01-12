using System;
using Dolittle.SDK.DependencyInversion;
using Dolittle.SDK.Tenancy;
using Microsoft.Extensions.DependencyInjection;

public interface ITenantScopedScopedService : IDisposable
{
}

[PerTenant(ServiceLifetime.Scoped)]
public class TenantScopedScopedService : TenantScopedService, ITenantScopedScopedService
{
    public TenantScopedScopedService(TenantId tenant) : base(tenant)
    {
    }
}
