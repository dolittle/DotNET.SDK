// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Customers;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Handling;
using Microsoft.Extensions.Logging;

public interface ISomeTenantScopedScopedService : IDisposable
{
    void Log(string s);
}

public class TenantScopedScopedService : ISomeTenantScopedScopedService
{
    readonly ILogger<TenantScopedScopedService> _logger;

    public TenantScopedScopedService(ILogger<TenantScopedScopedService> logger)
    {
        
        Console.WriteLine("Created TenantScopedScopedService");
        _logger = logger;
    }

    public void Log(string s)
    {
        _logger.LogInformation(s);
    }

    public void Dispose()
        => Console.WriteLine("Disposed TenantScopedScopedService");
}

public interface IScopedGloablService : IDisposable
{
}

public class ScopedGlobalService : IScopedGloablService
{
    public ScopedGlobalService(ILogger<ScopedGlobalService> logger)
    {
        Console.WriteLine("Created ScopedGlobalService");
    }

    public void Dispose()
        => Console.WriteLine("Disposed ScopedGlobalService");
}

public interface ISingletonGloablService : IDisposable
{
}

public class SingletonGlobalService : ISingletonGloablService
{
    public SingletonGlobalService(ILogger<SingletonGlobalService> logger)
    {
        Console.WriteLine("Created SingletonGlobalService");
    }

    public void Dispose()
        => Console.WriteLine("Disposed SingletonGlobalService");
}

[EventHandler("86dd35ee-cd28-48d9-a0cd-cb2aa11851cb")]
public class DishEater
{
    readonly ISomeTenantScopedScopedService _s;

    public DishEater(ISomeTenantScopedScopedService s, IScopedGloablService scopedGloablService, ISingletonGloablService x)
    {
        _s = s;
    }

    public void Handle(DishEaten @event, EventContext ctx)
    {
        _s.Log($"{ctx.EventSourceId} has eaten {@event.Dish}. Yummm!");
    }
}
