using System;
using Dolittle.SDK.Tenancy;

public interface ITenantScopedScopedService : IDisposable
{
}


public class TenantScopedScopedService : TenantScopedService, ITenantScopedScopedService
{
    public TenantScopedScopedService(TenantId tenant) : base(tenant)
    {
    }
}
