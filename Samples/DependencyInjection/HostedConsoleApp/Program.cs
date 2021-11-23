// See https://aka.ms/new-console-template for more information

using Dolittle.SDK.Samples.DependencyInjection.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder()
    .UseServiceProviderFactory(new Dolittle.SDK.DependencyInversion.ServiceProviderFactory())
    .ConfigureServices(services =>
    {
        services.AddSingleton(typeof(ISingleton), typeof(Singleton));
        services.AddTransient(typeof(ITransient), typeof(Transient));
        services.AddScoped(typeof(IScoped), typeof(Scoped));
    })
    .ConfigureContainer<Dolittle.SDK.DependencyInversion.ContainerBuilder>(_ => _.AddTenantServices((tenant, services) => services.AddSingleton(typeof(ITenantSpecific), typeof(TenantSpecific))))
    .Build();


// var client = await DolittleClient
//     .ForMicroservice("cc0bbb90-9ead-43a5-a53d-c32a3105fd43")
//     .WithServices(host.)
//     .Connect()
//     .ConfigureAwait(false);
//
// await Task.WhenAll(host.RunAsync(), client.Start());
