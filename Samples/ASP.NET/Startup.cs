// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Dolittle.SDK;
using Dolittle.SDK.Events.Store;
using Dolittle.SDK.Tenancy;

namespace ASP.NET
{

    public class Startup
    {
        Client _client;

        public Startup()
        {
            _client = Client
                .ForMicroservice("f39b1f61-d360-4675-b859-53c05c87c0e6")
                .WithEventTypes(eventTypes => 
                {
                    eventTypes.Register<DishPrepared>();
                    eventTypes.Register<DishEaten>();
                })
                .WithEventHandlers(builder => 
                {
                    builder.RegisterEventHandler<DishHandler>();
                    builder.RegisterEventHandler<DishCustomerHandler>();
                })
                .Build();

        }


        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen();
            services.AddSingleton(_client);

            services.AddTransient<DishHandler>();
            services.AddTransient<DishCustomerHandler>();
            services.AddTransient<IEventStore>(sp => _client.EventStore.ForTenant(Container.CurrentExecutionContext.Tenant));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Payouts API"));
            }

            app.UseRouting();

            app.UseEndpoints(_ =>
            {
                _.MapControllers();
                _.MapDefaultControllerRoute();
            });

            _client
                .WithContainer(new Container(app.ApplicationServices))
                .Start();
        }
    }
}
