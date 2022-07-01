using System;
using Dolittle.SDK.Tenancy;
using Microsoft.Extensions.Logging;

public interface ITenantScopedTransientService : IDisposable
{
}

public class TenantScopedTransientService : TenantScopedService, ITenantScopedTransientService
{
    public TenantScopedTransientService(TenantId tenant, ILogger<TenantScopedTransientService> logger) : base(tenant)
    {
    }
}
