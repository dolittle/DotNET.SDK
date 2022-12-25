using System;
using Dolittle.SDK.DependencyInversion;
using Dolittle.SDK.Tenancy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public interface ITenantScopedSingletonService : IDisposable
{
}
[PerTenant(ServiceLifetime.Singleton)]
public class TenantScopedSingletonService : TenantScopedService, ITenantScopedSingletonService
{
    public TenantScopedSingletonService(ILogger<TenantScopedScopedService> logger, TenantId tenant) : base(tenant)
    {
    }
}
