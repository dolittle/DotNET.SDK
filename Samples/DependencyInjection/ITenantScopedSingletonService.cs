using System;
using Dolittle.SDK.Tenancy;

public interface ITenantScopedSingletonService : IDisposable
{
}

public class TenantScopedSingletonService : TenantScopedService, ITenantScopedSingletonService
{
    public TenantScopedSingletonService(TenantId tenant) : base(tenant)
    {
    }
}
