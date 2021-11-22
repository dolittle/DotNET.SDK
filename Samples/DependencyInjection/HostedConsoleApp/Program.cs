// See https://aka.ms/new-console-template for more information

using System.Threading.Tasks;
using Dolittle.SDK;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder()
    .UseServiceProviderFactory<Dolittle.SDK.DependencyInversion.ServiceProviderFactory>()
    .ConfigureContainer<Dolittle.SDK.DependencyInversion.ContainerBuilder>(_ => _.)
    .
    .Build();

// var client = await DolittleClient
//     .ForMicroservice("cc0bbb90-9ead-43a5-a53d-c32a3105fd43")
//     .WithServices(host.)
//     .Connect()
//     .ConfigureAwait(false);
//
// await Task.WhenAll(host.RunAsync(), client.Start());

await host.