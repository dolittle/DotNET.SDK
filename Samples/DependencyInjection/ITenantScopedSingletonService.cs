using System;
using Dolittle.SDK.Tenancy;
using Microsoft.Extensions.Logging;

public interface ITenantScopedSingletonService : IDisposable
{
}

public class TenantScopedSingletonService : TenantScopedService, ITenantScopedSingletonService
{
    public TenantScopedSingletonService(ILogger<TenantScopedScopedService> logger, TenantId tenant) : base(tenant)
    {
    }
}
