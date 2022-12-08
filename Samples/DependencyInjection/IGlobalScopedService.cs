using System;

public interface IGlobalScopedService : IDisposable
{
}

public class GlobalScopedService : Service, IGlobalScopedService
{
}
