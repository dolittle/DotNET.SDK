using System;

public interface IGlobalTransientService : IDisposable
{
}

public class GlobalTransientService : Service, IGlobalTransientService
{
}
