using System;
using Dolittle.SDK.DependencyInversion;
using Dolittle.SDK.Tenancy;
using Microsoft.Extensions.Logging;

public interface ITenantScopedTransientService : IDisposable
{
}

[PerTenant]
public class TenantScopedTransientService : TenantScopedService, ITenantScopedTransientService
{
    public TenantScopedTransientService(TenantId tenant, ILogger<TenantScopedTransientService> logger) : base(tenant)
    {
    }
}
