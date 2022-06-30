using System;
using Dolittle.SDK.Tenancy;

public interface ITenantScopedTransientService : IDisposable
{
}

public class TenantScopedTransientService : TenantScopedService, ITenantScopedTransientService
{
    public TenantScopedTransientService(TenantId tenant) : base(tenant)
    {
    }
}
