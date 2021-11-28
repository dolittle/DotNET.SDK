// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK;
using Dolittle.SDK.Extensions.AspNet;
using Dolittle.SDK.Samples.DependencyInjection.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
        services.AddTransient<ITransient, Transient>();
        services.AddScoped<IScoped, Scoped>();
        services.AddSingleton<ISingleton, Singleton>();
        services.AddDolittle(
            _ => { },
            _ => _.WithTenantServices((tenant, services) =>
            {
                services.AddSingleton<ITenantSpecific, TenantSpecific>();
                services.AddScoped<ITenantSpecificScoped, TenantSpecificScoped>();
            }));
        services.AddSwaggerGen();
        // services.Replace(new ServiceDescriptor(typeof(IControllerActivator), typeof(CustomControllerActivator), ServiceLifetime.Singleton));
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseDeveloperExceptionPage()
            .UseSwagger()
            .UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                options.RoutePrefix = string.Empty;
            })
            .UseDolittle()
            .UseHttpsRedirection()
            .UseRouting()
            .UseAuthorization()
            .UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
    }
}
