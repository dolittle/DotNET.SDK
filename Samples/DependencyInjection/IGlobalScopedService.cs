using System;
using Microsoft.Extensions.DependencyInjection;

public interface IGlobalScopedService : IDisposable
{
}

public class GlobalScopedService : Service, IGlobalScopedService
{
}
