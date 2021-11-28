using System;
using System.Linq;
using Dolittle.SDK;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Store;
using Dolittle.SDK.Extensions.AspNet;
using Dolittle.SDK.Projections;
using Dolittle.SDK.Projections.Store;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDolittle(SetupDolittle, _ => {});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

app.UseDolittle();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

app.MapPost("/commit/{msg}", async (HttpContext ctx, [FromServices]IEventStore eventStore, string msg) =>
{
    await eventStore.CommitEvent(new SomeEvent(msg), "Some event source", ctx.RequestAborted).ConfigureAwait(false);
});
app.MapGet("/events", async (HttpContext ctx, [FromServices]IProjectionStore projections) =>
{
    var currentStates = await projections.GetAll<AllEvents>(ctx.RequestAborted).ConfigureAwait(false);
    return currentStates.Select(_ => _.Value.State);
});

app.Run();

static void SetupDolittle(DolittleClientBuilder builder)
    => builder
        .WithEventTypes(_ => 
            _.Register<SomeEvent>())
        .WithProjections(_ =>
            _.RegisterProjection<AllEvents>());

[EventType("9ab762e8-5a73-4183-a054-a15d0f5136eb")]
record SomeEvent(string Message);

[Projection("8121218b-8549-4f68-887e-1987afb89fde")]
class AllEvents
{
    public int NumEvents => Events.Length;

    public SomeEvent[] Events { get; set; } = Array.Empty<SomeEvent>();

    [KeyFromEventSource]
    public void On(SomeEvent evt, ProjectionContext ctx)
        => Events = Events.Append(evt).ToArray();
}