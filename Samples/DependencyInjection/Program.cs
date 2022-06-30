// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using Dolittle.SDK;
using Dolittle.SDK.Extensions.AspNet;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder();

builder.Host
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .UseDolittle(configureClientConfiguration: (x) => x.WithTenantServices((tenant, services) => services
        .AddSingleton<ITenantScopedSingletonService, TenantScopedSingletonService>()
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
app.Services.GetService<IGlobalSingletonService>();
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

