using System;
using Microsoft.Extensions.Logging;

public interface IGlobalTransientService : IDisposable
{
}

public class GlobalTransientService : Service, IGlobalTransientService
{
    public GlobalTransientService(ILogger<TenantScopedTransientService> logger) {}
}
