using System;

public interface IGlobalSingletonService : IDisposable
{
}

public class GlobalSingletonService : Service, IGlobalSingletonService
{
}
