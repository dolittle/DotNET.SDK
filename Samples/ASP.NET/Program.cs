// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK;
using Dolittle.SDK.Extensions.AspNet;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDolittle(SetupDolittleClient, ConfigureDolittleClient);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseDolittle();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");

app.UseHttpsRedirection();
app.UseStaticFiles();

});

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();

static void SetupDolittleClient(DolittleClientBuilder builder)
{
    builder
        .WithAllAggregateRoots()
        .WithAllEmbeddings()
        .WithAllProjections()
        .WithAllEventHandlers()
        .WithAllEventTypes();
}
static void ConfigureDolittleClient(DolittleClientConfiguration config)
{
}