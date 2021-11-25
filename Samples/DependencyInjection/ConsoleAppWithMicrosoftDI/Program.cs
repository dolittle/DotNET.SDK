// See https://aka.ms/new-console-template for more information

using System;
using System.Runtime.Serialization;
using Dolittle.SDK;
using Dolittle.SDK.Samples.DependencyInjection.Shared;
using Dolittle.SDK.Tenancy;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddSingleton(typeof(ISingleton), typeof(Singleton));
services.AddTransient(typeof(ITransient), typeof(Transient));
services.AddScoped(typeof(IScoped), typeof(Scoped));
// services.AddOptions();
// services.ConfigureOptions<>()

// Method 1
// services.AddDolittle(_ => { }, _ => _.WithTenantServices(((tenant, collection) => collection.AddSingleton(typeof(ITenantSpecific), typeof(TenantSpecific)))));
var provider = services.BuildServiceProvider();
// var client = await provider.GetConnectedDolittleClient();
var client = await DolittleClient.Setup(_ => {}).Connect(_ => _.WithServiceProvider(provider).WithTenantServices((tenant, collection) => collection.AddScoped<ITransient, Transient>()));
var scopeFac = client.Services.ForTenant(TenantId.Development).GetRequiredService<IServiceScopeFactory>();

var idGenerator = new ObjectIDGenerator();

using (var scope = scopeFac.CreateScope())
{
    Console.WriteLine(idGenerator.GetId(scope.ServiceProvider.GetRequiredService<IScoped>(), out var _));
    Console.WriteLine(idGenerator.GetId(scope.ServiceProvider.GetRequiredService<ITransient>(), out var _));
    
}
Console.WriteLine();
using (var scope = scopeFac.CreateScope())
{
    Console.WriteLine(idGenerator.GetId(scope.ServiceProvider.GetRequiredService<IScoped>(), out var _));
    Console.WriteLine(idGenerator.GetId(scope.ServiceProvider.GetRequiredService<ITransient>(), out var _));
}

// client.Services.Get<ITenantSpecific>(TenantId.Development).SayHello();
// client.Services.Get<ISingleton>(TenantId.Development).SayHello();
// client.Services.Get<ITransient>(TenantId.Development).SayHello();
// client.Services.Get<IScoped>(TenantId.Development).SayHello();
// client.Services.GetRequiredService<ISingleton>().SayHello();
// client.Services.GetRequiredService<ITransient>().SayHello();
// client.Services.GetRequiredService<IScoped>().SayHello();
