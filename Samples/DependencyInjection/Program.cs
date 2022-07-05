// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Autofac.Extensions.DependencyInjection;
using Dolittle.SDK;
using Dolittle.SDK.DependencyInversion;
using Dolittle.SDK.Extensions.AspNet;
using Lamar.Microsoft.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder();

builder.Host
    // Use Autofac. No need to setup custom tenant container creator when using Autofac
    // .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    // Use Lamar
    // .UseLamar(_ => _.For<ICreateTenantContainers<Lamar.Container>>().Use<LamarTenantContainerCreator>())
    .UseDolittle(configureClientConfiguration: _ => _
        // Setup to use Lamar tenant containers
        // .WithTenantContainerCreator<Lamar.Container>()
        .WithTenantServices((tenant, services) => services
            .AddSingleton<ITenantScopedSingletonService, TenantScopedSingletonService>() // This will only work when using either Lamar or Autofac
            .AddScoped<ITenantScopedScopedService, TenantScopedScopedService>()
            .AddTransient<ITenantScopedTransientService, TenantScopedTransientService>()));
builder.Services
    .AddSingleton<IGlobalSingletonService, GlobalSingletonService>()
    .AddScoped<IGlobalScopedService, GlobalScopedService>()
    .AddTransient<IGlobalTransientService, GlobalTransientService>()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddControllers();
var app = builder.Build();
app.UseDolittle();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
});
app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.MapGet("tenant/singleton", async ([FromServices]ITenantScopedSingletonService service) => Console.WriteLine("Handling request with tenant scoped singleton service"));
app.MapGet("tenant/scoped", async ([FromServices]ITenantScopedScopedService service) => Console.WriteLine("Handling request with tenant scoped scoped service"));
app.MapGet("tenant/transient", async ([FromServices]ITenantScopedTransientService service) => Console.WriteLine("Handling request with tenant scoped transient service"));
app.MapGet("global/singleton", async ([FromServices]IGlobalSingletonService service) => Console.WriteLine("Handling request with global singleton service"));
app.MapGet("global/scoped", async ([FromServices]IGlobalScopedService service) => Console.WriteLine("Handling request with global scoped service"));
app.MapGet("global/transient", async ([FromServices]IGlobalTransientService service) => Console.WriteLine("Handling request with transient service"));

app.Run();
