# [9.1.0] - 2021-7-14 [PR: #65](https://github.com/dolittle/DotNET.SDK/pull/65)
## Summary

Adds a new feature, Embeddings! They are similar to Projections, but they are meant to be used to event source changes coming from an external system. Check the [sample](https://github.com/dolittle/DotNET.SDK/tree/master/Samples/Tutorials/Embeddings) for an example.

### Added

- Embeddings! You can use them inline with the `WithEmbeddings()` builder, or by specifying the `ResolveUpdateToEvents`, `ResolveDeletionToEvents` and `On` methods or attributes for a class. The embeddings can be updated, deleted and fetched from the `client.Embeddings` property.


# [9.0.0] - 2021-6-16 [PR: #61](https://github.com/dolittle/DotNET.SDK/pull/61)
## Summary

Changes the behavior of the pinging system to be more reliable and to be ready to receive pings immediately upon connecting to the Runtime. This is to deal with a bug that was causing connections between the SDK and the Runtime to be dropped. This is a **breaking behavioral change** and it's related to the [release of version `6.0.0`](https://github.com/dolittle/Runtime/pull/532) of the Runtime. You have to update to version `6.*` of the Runtime, older versions wont work with this release of the SDK. For this we've added a [compatibility table](https://dolittle.io/docs/reference/runtime/compatibility).

Also adds a new way of specifying the ping interval period, which defines how often the client expects a ping from the Runtime. If the Runtime fails to ping after 3 attempts, the client will disconnect. The default value of the interval is 5 seconds.

### Added

- You can now specify the ping interval during client building with the `WithPingInterval(TimeSpan)` call. By default it's 5 seconds.

### Changed

- The reverse call connections are now ready to start receiving pings and writing pong immediately upon connecting.
- Changed back to the old implementation of the reverse call clients without RX.

### Fixed

- Event Horizon connections will now keep retrying forever properly.
- Pinging system should now timeout a lot less than before.


# [8.4.0] - 2021-4-9 [PR: #54](https://github.com/dolittle/DotNET.SDK/pull/54)
## Summary

Adds Projections, that are a special type of event handler dealing with read models. Projections can be defined either inline in the client build steps, or declaratively with `[Projection()]` attribute.

Example of writing a Projection inline and registering a declared one:
```csharp
var client = Client
    .ForMicroservice("f39b1f61-d360-4675-b859-53c05c87c0e6")
    .WithEventTypes(eventTypes =>
    {
        eventTypes.Register<DishPrepared>();
        eventTypes.Register<ChefFired>();
    })
    .WithProjections(projections =>
    {
        projections.CreateProjection("4a4c5b13-d4dd-4665-a9df-27b8e9b2054c")
            .ForReadModel<Chef>()
            .On<DishPrepared>(_ => _.KeyFromProperty(_ => _.Chef), (chef, @event, ctx) =>
            {
                chef.Name = @event.Chef;
                chef.Dishes.Add(@event.Dish);
                return chef;
            })
            .On<ChefFired>(_ => _.KeyFromProperty(_ => _.Chef), (chef, @event, ctx) =>
            {
                return ProjectionResult<Chef>.Delete;
            });
        projections.RegisterProjection<Menu>();
    })
    .Build();
```

Example of a declared projection:
```csharp
[Projection("0405b93f-1461-472c-bdc2-f89e0afd4dfe")]
public class Menu
{
    public List<string> Dishes = new List<string>();

    [KeyFromEventSource]
    public void On(DishPrepared @event, ProjectionContext context)
    {
        if (!Dishes.Contains(@event.Dish)) Dishes.Add(@event.Dish);
    }
}
```

Example of getting projections:
```csharp
var menu = await client.Projections
    .ForTenant(TenantId.Development)
    .Get<Menu>("bfe6f6e4-ada2-4344-8a3b-65a3e1fe16e9")
    .ConfigureAwait(false);

System.Console.WriteLine($"Menu consists of: {string.Join(", ", menu.State.Dishes)}");

var allChefs = await client.Projections
    .ForTenant(TenantId.Development)
    .GetAll<Chef>()
    .ConfigureAwait(false);

foreach (var chef in allChefs)
{
    System.Console.WriteLine($"Chef name: {chef.State.Name} and prepared dishes: {string.Join(",", chef.State.Dishes)}");
}
```

### Added

- New `client.WithProjections()` to build Projections inline in the clients build steps.
- Classes can be attributed with`[Projection('projectionId')]` to declare them as Projections (just like you can do with EventHandlers). The class itself becomes the readmodel for the projection.
- `On()` methods are the handlers for a Projection. They can be decorated with different attributes to declare the key to the projection.
- Get the state of a Projection with `client.Projections.Get<ReadModel>(key)` and `client.Projections.GetAll<ReadModel>()` (+ other overloads).
- Sample for how to use Projections in _Samples/Tutorials/Projections_.

### Changed

- Sample directory structure and moved the tutorials around


# [8.3.2] - 2021-3-24 [PR: #52](https://github.com/dolittle/DotNET.SDK/pull/52)
## Summary

Following async/await guidelines the RunContinuationsAsynchronously flag should always be set when creating TaskCompletionSource

### Changed

- Create all TaskCompletionSource with TaskCreationOptions.RunContinuationsAsynchronously


# [8.3.1] - 2021-2-22 [PR: #51](https://github.com/dolittle/DotNET.SDK/pull/51)
## Summary

The CorrelationId of the ExecutionContext was never changed, which means that everything would happen under the same CorrelationId. 

This fixes it by setting the CorrelationId to a new guid whenever doing ForTenant on EventStoreBuilder  


### Changed

- Create new execution context with random guid when doing ForTenant on EventStoreBuilder


# [8.3.0] - 2021-2-17 [PR: #50](https://github.com/dolittle/DotNET.SDK/pull/50)
## Summary

Adds the EventType property to the EventContext class that's used In event handlers and filters. I think that the EventType of the event to be handled should be easily accessible. I think it fits well inside the context of the event, EventContext. Since we can commit events in a raw format (with no association between Type and EventType) we need to have the ability to know the EventType of the event you are handing in a filter or event handler

### Added

- EventType Type read-only property on EventContext


# [8.2.0] - 2021-2-17 [PR: #49](https://github.com/dolittle/DotNET.SDK/pull/49)
## Summary

Adds the ability to register event handlers and event types from an Assembly.

### Added

- RegisterAllFrom(Assembly) to the EventHandlersBuilder and EventTypesBuilder


# [8.1.1] - 2021-2-14 [PR: #48](https://github.com/dolittle/DotNET.SDK/pull/48)
## Summary

Add more spec coverage around aggregates

### Added

- Specs around AggregateOf

### Changed

- Changed the structure a bit for specifications for Aggregates


# [8.1.0] - 2021-2-14 [PR: #36](https://github.com/dolittle/DotNET.SDK/pull/36)
## Summary

Adds aggregates to the SDK in a barebones matter, agnostic to whether there is IoC or not.

### Added

- System for working with aggregates and aggregate roots
- AggregateOf<> method on Client for creating an aggregate root operation on a specific aggregate
- Small sample showcasing the aggregates system
- Specs around AggregateRoot

### Changed
- docker-compose in samples run with latest runtime versions


# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [8.0.0] - 2020-10-28
### Added
- Commit for aggregate
- Fetch from aggregate

### Changed
- Change builder APIs to be more fluent
- Wait() to Start() on Client
- IContainer must know about ExecutionContext
- Event processors are now registered when starting the client

### Fixed
- IoC was broken for event processors

## [7.0.0] - 2020-10-28
### Changed
- Change EventHandlerBuilder API so that you can't accidentally overwrite your previous handlers

