// See https://aka.ms/new-console-template for more information

using System;
using Dolittle.SDK;
using Dolittle.SDK.Tenancy;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddSingleton(typeof(ISingleton), typeof(Singleton));
services.AddTransient(typeof(ITransient), typeof(Transient));
services.AddScoped(typeof(IScoped), typeof(Scoped));

var client = await DolittleClient
    .ForMicroservice("cc0bbb90-9ead-43a5-a53d-c32a3105fd43")
    .WithServices(services)
    .WithTenantServices((tenant, services) => services.AddSingleton(typeof(ITenantSpecific), typeof(TenantSpecific)))
    .Connect()
    .ConfigureAwait(false);

client.Services.Get<ITenantSpecific>(TenantId.Development).SayHello();
client.Services.Get<ISingleton>(TenantId.Development).SayHello();
client.Services.Get<ITransient>(TenantId.Development).SayHello();
client.Services.Get<IScoped>(TenantId.Development).SayHello();
client.Services.GetRequiredService<ISingleton>().SayHello();
client.Services.GetRequiredService<ITransient>().SayHello();
client.Services.GetRequiredService<IScoped>().SayHello();
await client.Start().ConfigureAwait(false);

public interface IService
{
    void SayHello();
}
public interface ISingleton : IService {}
public interface ITransient : IService {}
public interface IScoped : IService {}

public class Singleton : ISingleton
{
    public void SayHello()
        => Console.WriteLine($"Hello from Singleton");
}

public class Transient : ITransient
{
    public void SayHello()
        => Console.WriteLine($"Hello from Transient");
}
public class Scoped : IScoped
{
    public void SayHello()
        => Console.WriteLine($"Hello from Scoped");}

public interface ITenantSpecific : IService { }

public class TenantSpecific : ITenantSpecific
{
    readonly TenantId _tenant;

    public TenantSpecific(TenantId tenant)
    {
        _tenant = tenant;
    }

    public void SayHello()
        => Console.WriteLine($"Hello from tenant {_tenant}");
}
