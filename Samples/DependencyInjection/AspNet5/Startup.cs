// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Threading.Tasks;
using Dolittle.SDK;
using Dolittle.SDK.DependencyInversion;
using Dolittle.SDK.Samples.DependencyInjection.Shared;
using Dolittle.SDK.Tenancy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Swashbuckle.SwaggerUi;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddDolittle(
            _ => { },
            _ => _.WithTenantServices((tenant, services) => services.AddSingleton(typeof(ITenantSpecific), typeof(TenantSpecific))));
        services.AddSwaggerGen();
        services.Replace(new ServiceDescriptor(typeof(IControllerActivator), typeof(CustomControllerActivator), ServiceLifetime.Singleton));
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            options.RoutePrefix = string.Empty;
        });

        app.UseHttpsRedirection();
        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
        // app.UseMiddleware<>()
        app.Use(async (context, next) =>
        {
            Console.WriteLine("In middleware");
            await next();
        });
    }
}

