# [14.1.0] - 2022-1-28 [PR: #124](https://github.com/dolittle/DotNET.SDK/pull/124)
## Summary

Adds the possibility to take upon a specific projection read model as a dependency and use that to get the projection states for that read model type.

### Added

- `IProjectionOf<TReadModel>` that acts as a minimal `IProjectionStore` for a particular projection type.
- `IProjectionStore.Of<TReadModel>(...)` method with overloads for sending in `ProjectionId` and `ScopeId` to create instances of `IProjectionOf<TReadModel>`
- `IProjectionOf<TReadModel` is registered in the tenant scoped service providers for all types with the `[Projection]` attribute, or projections created in the `.WithProjections(...)` builder. So they can be injected in controllers etc.
-


# [14.0.0] - 2022-1-25 [PR: #119](https://github.com/dolittle/DotNET.SDK/pull/119)
## Summary

Simplifies the `IProjectionStore` apis by removing the `CurrentState<>` wrapper around the returned types from `Get(...)` and `GetAll(...)` methods, and returns an `IEnumerable<>` instead of an `IDicationary<,>` when getting multiple read models. And introduces a new `GetState(...)` method that has the wrapped types for when it is interesting.

### Added

- `IProjectionStore.GetState(...)` method that keeps the syntax of the previous `.Get(...)` method.

### Changed

- `IProjectionStore.Get(...)` returns the specified type or `object` instead of wrapping with `CurrentState<>`
- `IProjectionStore.GetAll(...)` returns an `IEnumerable<>` of the specified type or `object` instead of an `Dictionary<,>` of keys to `CurrentState<>`.


# [13.0.1] - 2022-1-24 [PR: #116](https://github.com/dolittle/DotNET.SDK/pull/116)
## Summary

Uses the new batch streaming method to get all Projection states from the Runtime. This fixes an issue where a large amount of Projection states caused the gRPC client in the SDK to throw an exception because the response message was too big.

### Fixed

- The `IProjectionStore.GetAll` method now uses the new gRPC method that streams results back in batches to fix the issue of too large gRPC messages when a large amount of projection read models have been created.


# [13.0.0] - 2022-1-20 [PR: #103](https://github.com/dolittle/DotNET.SDK/pull/103)
## Summary

Major improvements to the Dolittle Client, in how it connects to the Runtime, configuration, setup and integrations with ASP.Net Core. Combined these changes aim to make the SDK easier to setup and configure, and to make it easier to detect when incompatible versions are used.

### Added

- Support for Dependency Injection using Microsoft Dependency Injection internally, also supporting tenant-specific bindings.
- The DolittleClient and tenant specific resources (IEventStore, IAggregates, IProjections, ...) are bound in the service provider used and exposed by the client. They can be used in e.g. Event Handlers, or with the AspNetCore integration in Controllers.
- AspNetCore integration by adding `.UseDolittle()` on both the host and application builder that uses the Microsoft Configuration system, starts the DolittleClient as a hosted service, and a middleware that sets the Request service provider based on the `Tenant-ID` header (provided by the platform). See the AspNetCore sample.
- When starting up a DolittleClient, it now performs an initial handshake with the configured Runtime to determine that the versions of the SDK and the Runtime are compatible, and retrieves the MicroserviceId to configure its execution context (provided by the platform).

### Changed

- Building a DolittleClient has been split into two steps, namely `.Setup()` and `.Connect()`, to make integrations easier.
- The automatic discovery of types and processors is now enabled by default.
- The configured Tenants are retrieved during the first connection to the Runtime, so the `.Tenants` on the DolittleClient is no longer an asynchronous call.
- The builder APIs exposed in the `.Setup(...)` call have been changed so they are all called `.Register(...)` or `.Create(...)`.
- The `AggregateOf` methods on the client have been changed to an `Aggregates` property that behaves more like the other tenant specific resources.

### Fixed

- The SDK de-duplicates registered types and processors (Event Handlers, ...) so that you can use both automatic discovery and manual registration.

### Removed
 - The builders exposed in the `.Setup(...)` call have been changed to interfaces that don't expose the internal `.Build(...)` method.


# [12.0.0] - 2021-11-18 [PR: #100](https://github.com/dolittle/DotNET.SDK/pull/100)
## Summary

Adds the ability to get the configured tenants, and a MongoDB database per tenant from the Runtime through the client. Also renames the Client to DolittleClient, and introduces interfaces many places to simplify creation of mocks for testing purposes.

### Added

- ITenants Tenants property on the client for getting all tenants from the Runtime
- IResourcesBuilder Resources property on the client for getting resources for a specific tenant. Currently supports MongoDB. 
- IDolittleClient interface that DolittleClient implements, and interfaces for other classes in the client structure

### Changed

- Client renamed to DolittleClient
- ClientBuilder renamed to DolittleClientBuilder


# [11.0.0] - 2021-11-5 [PR: #77](https://github.com/dolittle/DotNET.SDK/pull/77)
## Summary

Fixes a problem with aggregates so that it now can be used in an async way. Registration of event types and aggregate roots to the Runtime.


### Added

- Registration of alias for event types through the attribute or builder
- Registration of alias fro aggregate roots through the attribute
- Default alias registration for event type classes and aggregate root classes. Default is the name of the class
- Extension Methods on the ClientBuilder for registering all event handlers, event types, projections, embeddings, aggregate roots by discovering them through the assemblies and DLLs in the solution 

### Changed

- Log Level for registered Event Processors to Information
- The Getting Started, Aggregates, Projections and Embeddings tutorials updated to reflect new features of the SDK

### Fixed

- A problem where you couldn't use an aggregate root in an asynchronous manner


# [10.1.0] - 2021-10-21 [PR: #76](https://github.com/dolittle/DotNET.SDK/pull/76)
## Summary

Updates Grpc, protobuf and contracts dependency versions and adds the possibility to register event handlers with aliases that is useful for when using the Dolittle CLI.

### Added

- `WithAlias` build step on the fluent builder for event handlers.
- `alias` argument on the `EventHandler` attribute
- Event handler classes without the `alias` argument gets registered with an alias that is the class name.

### Changed

- Updated Grpc, protobuf and contracts dependency versions


# [10.0.0] - 2021-10-13 [PR: #73](https://github.com/dolittle/DotNET.SDK/pull/73)
## Summary

Implementing the changes introduced by https://github.com/dolittle/Contracts/pull/53. Allowing EventSourceID and PartitionID to be strings, to more easily integrate with events from existing systems.

This is considered a breaking change because it requires a Runtime compatible with Contracts v6 to function.

### Added

- EventSourceID is now a string instead of a Guid.
- PartitionID is now also a string instead of a Guid.

### Fixed

- Aligned names of event type fields throughout messages from Contracts v6.0.0


# [9.2.0] - 2021-9-29 [PR: #72](https://github.com/dolittle/DotNET.SDK/pull/72)
## Summary

This PR is combined with PR #71 

Adding the ability to set a default `JsonSerializerSettings` instance for the serialization and deserialization of events. This allows for completely custom settings e.g. adding converters for types or casing configuration or similar. Fixes #70.

Using it would then be as follows during the building of the client:

```csharp
client
   .ForMicroservice(...)
   .WithJsonSerializerSettings(new JsonSerializerSettings { Converters = .... });
```

### Added

- Ability to set a default `JsonSerializerSettings` instance.
- `EventContentSerializer` honoring the default `JsonSerializerSettings`.


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

