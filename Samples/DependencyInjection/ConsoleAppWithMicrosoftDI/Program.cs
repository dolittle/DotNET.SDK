// See https://aka.ms/new-console-template for more information

using Dolittle.SDK;
using Dolittle.SDK.Samples.DependencyInjection.Shared;
using Dolittle.SDK.Tenancy;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddSingleton(typeof(ISingleton), typeof(Singleton));
services.AddTransient(typeof(ITransient), typeof(Transient));
services.AddScoped(typeof(IScoped), typeof(Scoped));

// Method 1
services.AddDolittle(_ => { }, _ => _.WithTenantServices(((tenant, collection) => collection.AddSingleton(typeof(ITenantSpecific), typeof(TenantSpecific)))));
var provider = services.BuildServiceProvider();

var client = await provider.GetRequiredService<IDolittleClient>()
    .Connect(provider.GetRequiredService<DolittleClientConfiguration>()).ConfigureAwait(false);

// Method two
// var client = await DolittleClient
//     .Setup(_ => { })
//     .Connect(_ => _
//         .WithServiceProvider(provider)
//         .WithTenantServices((tenant, services) =>
//         {
//             services.AddSingleton(typeof(ITenantSpecific), typeof(TenantSpecific));
//         })
//     ).ConfigureAwait(false);


client.Services.Get<ITenantSpecific>(TenantId.Development).SayHello();
client.Services.Get<ISingleton>(TenantId.Development).SayHello();
client.Services.Get<ITransient>(TenantId.Development).SayHello();
client.Services.Get<IScoped>(TenantId.Development).SayHello();
client.Services.GetRequiredService<ISingleton>().SayHello();
client.Services.GetRequiredService<ITransient>().SayHello();
client.Services.GetRequiredService<IScoped>().SayHello();
