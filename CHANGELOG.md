# [23.5.0] - 2024-10-23 [PR: #247](https://github.com/dolittle/DotNET.SDK/pull/247)
# Summary
This adds support for retroactively redacting personal data details from previously committed events. Redactions are scoped to a single EventSourceId, and a single event type, and allow you to overwrite or remove specific fields. 

## How to use it

Events that should be redacted must be annotated to target the fields that should be updated or removed

```csharp
[EventType("24e3a119-57d5-45d7-b7ef-a736fe6331e7")]
public class CustomerRegistered
{
    // The generic type will replace the value
    [RedactablePersonalData<string>("<redacted>")]
    public string Name { get; init; }
    
    //  The non-generic one will remove the field altogether
    [RedactablePersonalData]
    public Address? CustomerAddress { get; init; }
}
```

They can then be redacted by committing a targeted redaction event on the same eventsourceId as the redacted events.

It can either be done with the built-in event

```csharp
    public void GdprForget(string reason, string performedBy)
    {
        Apply(Redactions.Create<CustomerRegistered>(reason, performedBy));
    }
```

or a user-created event

```csharp
[EventType("de1e7e17-bad5-da7a-a4f7-e409ae1785dc")]
public class CustomerDetailsForgotten : PersonalDataRedactedForEvent<CustomerRegistered>
{
}
```
Applied like this
```csharp
    public void GdprForget(string reason, string performedBy)
    {
        Apply(Redactions.Create<CustomerRegistered, CustomerDetailsForgotten>(reason, performedBy));
    }
```

## What about read models and aggregate state ?

Aggregates and read models should handle the redactions as normal events, and update state accordingly.

Ex
```csharp
    public void On(CustomerDetailsForgotten evt)
    {
        _address = null;
        _name = null;
        _gdprRedacted = true;
    }
```

Read models are not automagically updated, but should handle the redaction events themselves. Redaction events after being committed behave exactly as any other event, and can be processed by both eventhandlers and projections.

Since you would want the eventhandlers to only handle as few events as needed, user defined redaction events are the suggested way to perform redactions.





## How does it work?
The runtime will recognize the redaction event type with the redaction prefix `"de1e7e17-bad5-da7a"`, and if the events have the correctly formatted structure, it will perform the redactions in the same transaction as the new events are being added.

It will replace overridden properties as specified in `RedactedProperties`, and remove properties that are defined with null values.

Redaction events themselves cannot be redacted, to ensure auditability of the system.

### Added

- `Dolittle.SDK.Events.Redaction.PersonalDataRedactedForEvent` - Built in event that can redact any event type
- `Dolittle.SDK.Events.Redaction.PersonalDataRedactedForEvent<TEvent>` - Class to extend in order to create redactions for specific events
- `Dolittle.SDK.Events.Redaction` - Use this to simplify creation of redaction events

### Updated
- The solution was updated to use [centralized package management](https://learn.microsoft.com/en-us/nuget/consume-packages/Central-Package-Management) to simplify dependency maintenance. Dependencies have also been upgraded.


# [23.4.0] - 2024-10-2 [PR: #246](https://github.com/dolittle/DotNET.SDK/pull/246)
## Summary

Allow projection read models to get dependencies from `IServiceProvider`  By extending `IRequireDependencies<T>`. It will then get a callback when initialized, and can resolve external dependencies scoped to its current tenant. This can enable logging use cases on init, mutations etc.

### Added
`Dolittle.SDK.Projections.IRequireDependencies<T>`, interface for read models to be able to inject dependencies


# [23.3.0] - 2024-7-31 [PR: #245](https://github.com/dolittle/DotNET.SDK/pull/245)
## Summary

This release is primarily focused on update and maintenance of SDK dependencies. 

In addition it has updates to the aggregate test helpers.

### Changed
- Aggregate test helpers refactored
  - Changed `ShouldHaveEvent` -> `ShouldHaveEvents`, to assert on potentially several events of a type
  - Added new `ShouldHaveEvent` to assert on a single event of a type

### Removed
- Removed dependency on `BaselineTypeDiscovery`.


# [23.2.5] - 2024-6-20 [PR: #244](https://github.com/dolittle/DotNET.SDK/pull/244)
## Summary

This patch adds analyzers to verify that mutations do not throw exceptions. In addition it adds a few new helper methods to aggregate test classes

### Added
- Diagnostic: `ExceptionInMutation`
- AggregateRootTests exception helpers


# [23.2.4] - 2024-6-13 [PR: #243](https://github.com/dolittle/DotNET.SDK/pull/243)
## Summary

Updates analyzers to make sure current time is not being used to populate read models or hydrate aggregate state.
Using current time will cause the state to no longer be idempotent, and breaks the abstraction. This also adds a code fix to change the time provider to be the time when the event was committed.

### Added
- Analysis & Codefix for projections
- Analysis for aggregate roots.


# [23.2.3] - 2024-6-10 [PR: #242](https://github.com/dolittle/DotNET.SDK/pull/242)
## Summary

Adds a few new test helpers to validate exceptional behavior in aggregates. (`VerifyThrows` & `VerifyThrowsExactly`)

Added example project demonstrating how to use test helpers.




### Added
- Test helpers `VerifyThrows` & `VerifyThrowsExactly`
- Example project `ASP.NET.Tests`


# [23.2.2] - 2024-6-2 [PR: #241](https://github.com/dolittle/DotNET.SDK/pull/241)
## Summary

Improved documentation on the test helper methods. 
Minor test lib refactoring


### Added
- `EventValueAssertion.AndThat`
- `ReadModelValueAssertion.AndThat`


# [23.2.1] - 2024-5-16 [PR: #240](https://github.com/dolittle/DotNET.SDK/pull/240)
## Summary
- Ensure proto.cluster messages wait for system readiness to begin processing. Previously it could start before Proto.Cluster had started, and logged errors. This did not affect the projection result itself, as the projection would be processed in a retry.
- Added projection tracing
- Updated OTEL & Grpc dependencies

Reduced OTEL resource usage:
- Skip send activity / span for Proto.Actor traces for root context.


# [23.2.0] - 2024-3-14 [PR: #239](https://github.com/dolittle/DotNET.SDK/pull/239)
## Summary
Added the ability to use projections keyed both on event properties, eventSourceId and when the event occurred.

This enables projections to be used for aggregations over time, and to optimize read  model for specific statistics.

In addition there has been added the ability to key by function, which gives complete control over which id to target

### Added
- `KeyFromPropertyAndOccurredAttribute`
- `KeyFromEventSourceAndOccurredAttribute`
- `KeyFromFunctionAttribute`


# [23.1.0] - 2024-3-12 [PR: #238](https://github.com/dolittle/DotNET.SDK/pull/238)
## Summary
This release focuses on Projection / read model features and performance.  

In addition to new Roslyn analyzers, this adds the ability to have fine grained control over TTL for each projection type, and to select if it should go via in-memory cache when getting projections.

There has also been added `IProjectionOf.Subscribe`, which enables a client to have real time updates on individual keys. Useful for streaming changes to frontend / UX when a specific read model has focus.

### Added
- Aggregates mutation method codefixes
- Projection analyzers & codefixes
- Ability to configure TTL for read model cache per entity
- Ability to read through read model cache when getting individual projections
- Projection subscriptions: Ability to get all updates to an individual key. Requires the projection to be ICloneable, as the classes are not guaranteed to be immutable.


### Fixed
- Projection idempotency, will now track which offset was last processed. This guarantees each event is processed only once for a single read model.


# [23.0.0] - 2024-3-8 [PR: #237](https://github.com/dolittle/DotNET.SDK/pull/237)
## Summary

### Projections

This version rewrites projections from being a runtime feature to being done completely on the SDK side. It keeps the interface pretty much the same as the old projections, but relaxes some rules around method signatures.  Context is now an optional second parameter, and allows both `ProjectionContext `or `EventContext`. KeyFrom is now optional and will default to the `EventSourceId`. All projections also needs to extend 

The existing IProjectionOf and IProjectionStore interfaces have been reduced to expose `Get` and the ability to use `IQueryable`, and will now allow all filtering to be done in the DB.

Example Projection

```csharp
[Projection("055319f1-6af8-48a0-b190-323e21ba6cde")]
public class SomeProjection : ReadModel
{
    public int UpdateCount { get; set; }
    public string Content { get; set; } = string.Empty;
    public int SomeNumber { get; set; }

    public void On(SomeEvent evt, ProjectionContext ctx)
    {
        UpdateCount++;
        Content = evt.Content;
    }

    public void On(SomeOtherEvent evt, ProjectionContext ctx)
    {
        UpdateCount++;
        SomeNumber = evt.SomeNumber;
    }

    public ProjectionResultType On(DeleteEvent _) => ProjectionResultType.Delete;
}
```



### Aggregates
Added `Perform<TResponse>` which allows the caller to get return values from operations on aggregates.


### Added

- SDK Projections - Full rewrite
- `ProjectionTests<TProjection>` - Added blackbox testing of projections.
- `AggregateRoot.Perform<TResponse>` - Ability to get responses from aggregates

### Removed

- Runtime integrated projections


# [22.1.3] - 2024-2-26 [PR: #235](https://github.com/dolittle/DotNET.SDK/pull/235)
## Summary
Maintenance release - Updated library dependencies

### Changed

- Updated OTEL to 1.7
- Updated Proto.Actor to 1.6
- Updated Microsoft extensions to 8.0
- Updated Mongo drivers to 2.24
- Updated Grpc & Protobuf to latest


# [22.1.2] - 2023-10-24 [PR: #234](https://github.com/dolittle/DotNET.SDK/pull/234)
## Summary

Maintenance release, upgrading SDK dependencies.


# [22.1.1] - 2023-10-4 [PR: #233](https://github.com/dolittle/DotNET.SDK/pull/233)
## Summary
Minor tracing improvements.

### Added
 * Added event metadata to eventhandler traces

### Removed
*  HandleEventRequests traces (already traced at a lower level)


# [22.1.0] - 2023-9-18 [PR: #232](https://github.com/dolittle/DotNET.SDK/pull/232)
## Summary

Adds support for new features from runtime V9.1. This includes the ability to configure event handlers to start from the latest events instead of the beginning (startFrom), as well as being able to limit the handler to events produced within a specific timeframe. (startFromTimestamp, stopAtTimestamp)

In addition this release comes with new analyzers for event handlers, making sure that incorrect use of the handlers should be a compile time issue instead of a runtime one. 

### Added

- EventHandlerAttribute: startFrom (Earliest / Latest)
- EventHandlerAttribute: startFromTimestamp - Overrides startFrom if set, ensures the handlers starts on the first event after the given timestamp. It can be in the future.
- EventHandlerAttribute: stopAtTimestamp - If set, the event handler will stop processing when it reaches events newer than this timestamp.
- Additional Roslyn analyzers, to ensure correct use of EventHandlers / aggregates


# [22.0.5] - 2023-9-7 [PR: #229](https://github.com/dolittle/DotNET.SDK/pull/229)
## Summary

This solves for timing issues with VERY large aggregates. Normally this will not be necessary, but if aggregates are very long and take a long time to rehydrate, this will let them still work correctly. In addition, added `DefaultAggregatePerformTimeout `which can be set globally to time out perform requests if they are extremely slow. 

### Added
- `DefaultAggregatePerformTimeout`

### Removed
- Duplicate tracing of event handlers


# [22.0.4] - 2023-8-28 [PR: #228](https://github.com/dolittle/DotNET.SDK/pull/228)
## Summary

Added analyzer to guard against bugs where new events are produced in an `On` method. These should only update the internal state of the aggregate, not produce new events.

### Added

- Check against `Apply` used in aggregate `On` methods


# [22.0.3] - 2023-8-15 [PR: #227](https://github.com/dolittle/DotNET.SDK/pull/227)
## Summary
Reduces some overhead by reducing duplicate tracing of event handling. Fixed an issue where both the client and actor would wrap the exception in AggregateRootOperationFailed

### Changed
- Removed duplicate event traces (avoid tracing HandleEventRequest).

### Fixed
- Avoids wrapping AggregateRootOperationFailed in an AggregateRootOperationFailed.


# [22.0.2] - 2023-8-8 [PR: #226](https://github.com/dolittle/DotNET.SDK/pull/226)
## Summary

Simplify usage of `AggregateRootTests<T>`, no longer requiring the user to provide the callback to create the aggregate instance.

### Added
- Allow AggregateRootTests to be used without providing callback to instantiate aggregate instance


# [22.0.1] - 2023-8-8 [PR: #225](https://github.com/dolittle/DotNET.SDK/pull/225)
## Summary
Minor Dolittle.SDK.Testing improvements

### Added
- Added ability to access the events being asserted on in `EventValueAssertion<T>`.

### Fixed
- Fixed an issue with `EventSourceId` not being set during testing when using aggregate roots with default constructor.


# [22.0.0] - 2023-6-22 [PR: #224](https://github.com/dolittle/DotNET.SDK/pull/224)
## Summary
Major SDK release supporting runtime V9.0.0. In addition to V9 having major performance improvements, this allows the user to also enable concurrent processing with partitioned event handlers.

### Added
- Support for concurrent event handlers
- [Metrics via OpenTelemetry](https://github.com/dolittle/DotNET.SDK/pull/221)

### Changed
- Improved eventhandler disconnect procedure


# [21.0.1] - 2023-6-8 [PR: #220](https://github.com/dolittle/DotNET.SDK/pull/220)
## Summary
Removed retries against aggregate actors. Added new test classes for Aggregates.

### Added
- Additional helpers / assertions for testing aggregate roots

### Fixed
- Removed potential retries on Aggregate.Perform, which could occur when response times are greater than the set retry timeout.


# [21.0.0] - 2023-6-2 [PR: #219](https://github.com/dolittle/DotNET.SDK/pull/219)
## Summary

Release Highlights: MongoDB Client Config Override, Enhanced MongoDB Tracing, Library Upgrades, and Embeddings API Removal

### Added

- [Allow users to override MongoDB client config](https://github.com/dolittle/DotNET.SDK/pull/216)

### Changed

- [Default enabled filtering for MongoDB traces.](https://github.com/dolittle/DotNET.SDK/pull/218)
- Upgraded libraries for Proto.Actor, MongoDB & OpenTelemetry

### Removed

- [Embeddings API has been removed.](https://github.com/dolittle/DotNET.SDK/pull/217)


# [20.2.0] - 2023-1-23 [PR: #195](https://github.com/dolittle/DotNET.SDK/pull/195)
## Summary

Adds support for testing of Aggregate Roots in the Dolittle.SDK.Testing package (need to be referenced directly in test projects). Also adds a couple more quality-of-life tools to the SDK like MongoDB BSON serializer for ConceptAs types.

### Added

- Dolittle.SDK.Testing package with tools for testing Aggregate Roots
- MongoDB Bson serializer for ConceptAs types
- `WithMongoDatabaseSettings` method on Dolittle configuration builder for configuring `MongoDatabaseSettings`

### Fixed
- A bug with `PerTenant`-attribute where open-generic services would not be registered correctly


# [20.1.0] - 2022-12-25 [PR: #194](https://github.com/dolittle/DotNET.SDK/pull/194)
## Summary

(See previous PR for changes https://github.com/dolittle/DotNET.SDK/pull/193)
Make it possible to automatically discover and register tenant-scoped services. This is not meant to be a replacement for the 
`WithTenantServices` method on the Dolittle configuration callback as that will still need to be used for services that requires more complicated registration, but the new attribute-based registration should suffice for most situations.
### Added

- `PerTenant` attribute that can be used to mark classes as PerTenant-scoped services. The `PerTenant` attribute has a couple of options in terms of `ServiceLifetime` (Transient by default) and whether the service should be registered as itself (off by default)


# [20.0.2] - 2022-12-13 [PR: #192](https://github.com/dolittle/DotNET.SDK/pull/192)
### Fixed

- Does not shortcut rehydration for stateless aggregates. Fixes invalid AggregateRootVersion for subsequent commits on stateless aggregates ( aggregates without mutations)


# [20.0.1] - 2022-12-12 [PR: #190](https://github.com/dolittle/DotNET.SDK/pull/190)
## Summary

Fixes a critical bug in the DolittleClient preventing all clients to crash.

### Fixed

- Bug in starting up DolittleClient


# [20.0.0] - 2022-12-8 [PR: #183](https://github.com/dolittle/DotNET.SDK/pull/183)
## Summary

Improves the overall developer experience in multiple ways and enables visibility of application errors in Studio through the Runtime.

### Added

- Roslyn Analyzers:
  - Identity Attributes
    - Added check that all identities are unique within the type
    - Added check that the identity is a valid Guid
    - Added code fixes to autogenerate valid Id's
  - Aggregates
    - Will check that every object used either in Apply or On is a valid event (with annotation)
    - Added code fix to add attributes to classes missing EventType
    - Added the ability to auto-generate On-method (mutation) for a given Apply
    - Added checks that the On-methods have the correct visibility
  - EventStore
    - Checks that objects being committed are valid events (with annotation)
    - Added code fix to add annotation if it is missing
- Sends application model build results to the Runtime in the handshake.

### Changed

- Multiple internal/infrastructure APIs to enable the new additions to the application model and build results


# [19.0.0] - 2022-11-25 [PR: #173](https://github.com/dolittle/DotNET.SDK/pull/173)
## Summary

Major performance improvements to aggregates. They are now sequentially handled in actors and kept in-memory for subsequent operations.
Upgraded to dotnet 7
Improved trace coverage with additional traced defaults (MongoDB, AspNetCore)

### Added

- Dotnet 7 builds
- Dependency on Proto.Cluster
- Additional tracing out of the box (MongoDB, AspNetCore)

### Fixed

- Concurrent operations on aggregates are no longer going to throw, and will instead be handled sequentially.

### Removed

- Dotnet 5 builds


# [18.1.1] - 2022-10-31 [PR: #164](https://github.com/dolittle/DotNET.SDK/pull/164)
## Summary

A recently added bug in the aggregate root that would occur for aggregate roots where the aggregate committed events that was not used to manipulate the state of the aggregate root.

### Fixed

- Set the correct aggregate root version at the end of rehydration.


# [18.1.0] - 2022-10-26 [PR: #163](https://github.com/dolittle/DotNET.SDK/pull/163)
## Summary

Enables dependency injection for aggregate roots

### Added

- Tenant scoped dependencies will now be injected in the aggregate roots

### Deprecated

- AggregateRoot base constructor with event source id is no longer necessary


# [18.0.1] - 2022-10-20 [PR: #162](https://github.com/dolittle/DotNET.SDK/pull/162)
## Summary

Fixes for a problem with the tenant scoped containers where it could not resolve tenant scoped dependencies registered as callbacks.

### Fixed

- Tenant Scoped Containers


# [18.0.0] - 2022-9-15 [PR: #160](https://github.com/dolittle/DotNET.SDK/pull/160)
## Summary

Adds a new method to the `EventStore` for fetching committed aggregate events filtered by event types. This allows us to change the rehydration of aggregates to be much more effective by just fetching the committed aggregate events for an aggregate that are relevant to the rehydration (meaning that there are an `On` method for that event type). This can have a significant impact on the performance of aggregates that have many events, but few state changes or are completely stateless. 

### Added

- `FetchForAggregate` that takes in a list event types used for filtering
- `FetchStreamForAggregate` fetches a stream of committed aggregate event batches

### Changed

- Rehydration logic of aggregate roots. It now only fetches the committed aggregate events that are relevant
- `AggregateRootVersion` on the `CommittedAggregateEvents` now represents the current aggregate root version of the aggregate root, not the version of the last committed aggregate event
- Minor version of Contracts meaning that this version of the SDK is only compatible with version `>= 8.5.0` of the Runtime


### Fixed

- Some users could experience exceptions when performing actions on aggregate roots that had lots of events or big events due to the protobuf messages being too big. This should be fixed now since the internals of fetching aggregate roots are now implemented using streaming and batching.


# [17.2.3] - 2022-9-9 [PR: #161](https://github.com/dolittle/DotNET.SDK/pull/161)
### Fixed

* Fixes bug in `WithOpenTelemetrySettings`


# [17.2.2] - 2022-8-26 [PR: #158](https://github.com/dolittle/DotNET.SDK/pull/158)
## Summary

We have experienced applications that were crashing due to grpc messages growing too big due to the volume of events for an aggregate root. As a quick workaround for this we can increase the maximum size of the grpc messages. In the long term a better solution will be to implement streaming and batch mechanisms.

### Changed

- The max size of grpc messages to 32 mb


# [17.2.1] - 2022-8-10 [PR: #156](https://github.com/dolittle/DotNET.SDK/pull/156)
## Summary

We noticed an issue with the `IsService` implementation of the Lamar IoC container not working as we expected. This caused an issue where services would not be resolved correctly. To fix this issue for Lamar we had to make the implementation a bit less optimized in that services will be created multiple times. 

### Fixed

- An issue in the tenant-specific dependency inversion container system that was present when using Lamar


# [17.2.0] - 2022-7-8 [PR: #149](https://github.com/dolittle/DotNET.SDK/pull/149)
## Summary

A problem with our tenant scoped service providers caused crashes when resolving tenant scoped singleton services with dependencies not present in the derived tenant scoped service provider directly, pluss other problems. 

### Fixed
- The DependencyInversion internals to fully support tenant child containers for all kinds of IServiceProvider using Autofac as the dependency injection engine

### Changed
- `IAggregateOf<>` registered as `transient` instead of `singleton` services 

### Added
- `ICreateTenantContainers<>` interface for hooking in your own dependency injection library of choice as the tenant containers. Look at Samples/DependencyInjection for example for setting up Lamar
- Configuration methods for setting up the custom tenant container creator


# [17.1.2] - 2022-7-7 [PR: #153](https://github.com/dolittle/DotNET.SDK/pull/153)
## Summary

Fixes a problem with Artifacts caused event types with specified Alias to cause exception when being handled by an event handler

### Fixed

- EventType with explicit Alias should now work.


# [17.1.1] - 2022-7-7 [PR: #143](https://github.com/dolittle/DotNET.SDK/pull/143)
## Summary
### Fixed

- A bug where if there were errors while adding decorated event handler methods it would not try to add convention event handler methods


# [17.1.0] - 2022-7-5 [PR: #145](https://github.com/dolittle/DotNET.SDK/pull/145)
## Summary

Sets up tracing by default using Open Telemetry and exporting with OTLP. The tracing configuration is configurable through code using the `.UseDolittle()` API. By default the OTLP endpoint is set with the `OTEL_EXPORTER_OTLP_ENDPOINT` environment variable. If that is not set it uses `http://localhost:4317`

### Added

- Automatic setup of tracing though OTLP by convention feeding traces and logs to endpoint set with the `OTEL_EXPORTER_OTLP_ENDPOINT` environment.
- Tracing on multiple important actions in the SDK like committing and handling events


# [17.0.2] - 2022-6-21 [PR: #147](https://github.com/dolittle/DotNET.SDK/pull/147)
## Summary

An issue with the previous release resulted in the nuget packages not being published.

### Fixed

- Release pipeline

### Changed

- Use whole exception as failure reason when processing an event fails.


# [17.0.1] - 2022-6-21 [PR: #146](https://github.com/dolittle/DotNET.SDK/pull/146)
## Summary

### Changed

- Use whole exception as failure reason when processing an event fails.


# [17.0.0] - 2022-3-25 [PR: #144](https://github.com/dolittle/DotNET.SDK/pull/144)
## Summary

Use the newest major version of the Dolittle Grpc Contracts to be compatible with version 8 of the Runtime

### Changed

- Use the newest major version 7 of the Dolittle Grpc Contracts


# [16.0.1] - 2022-3-22 [PR: #142](https://github.com/dolittle/DotNET.SDK/pull/142)
## Summary

Fixes an issue where event handlers with wrong signatures were not picked up early so that the developer was met with a confusing error message.


### Changed

- Significantly improves the error messages logged when having event handlers with wrong signatures.


# [16.0.0] - 2022-3-14 [PR: #136](https://github.com/dolittle/DotNET.SDK/pull/136)
## Summary

Updates grpc packages and uses a single grpc channel for all connections

### Changed

- Updated grpc version to 2.43.0
- Updated contracts version
- Uses one GrpcChannel for all gRPC connections. This is a potential breaking change for netcoreapp 3.1 since it does not allow multiple concurrent http2 connections for one GrpcChannel meaning that if there are 100 or more event processors in a netcoreapp 3.1 application it will start hanging. 

### Removed

- Removed the Contrib.M1 Grpc native executable for arm64. This is no longer needed with the new .NET Grpc packages


# [15.1.3] - 2022-3-8 [PR: #138](https://github.com/dolittle/DotNET.SDK/pull/138)
## Summary

Fixes a bug that resulted in wrong retry timings when processing of events failed.

### Fixed

- The event processing retry time should now increment in the correct interval


# [15.1.2] - 2022-3-1 [PR: #135](https://github.com/dolittle/DotNET.SDK/pull/135)
## Summary

Fixes a bug introduced in the previous release where building Event Handlers would cause a stack overflow because an `Equals(...)` method called itself recursively.

### Fixed

- A bug caused clients that was using Event Handlers to crash with a stack overflow when while setting up the Dolittle Client, because of a recursive call to `Equals(...)` in event handler builders while checking the model.


# [15.1.1] - 2022-2-11 [PR: #133](https://github.com/dolittle/DotNET.SDK/pull/133)
## Summary

Fixes an issue where an event processor would be re-registered immediately if reverse call client was cancelled by the server.

### Fixed

- Waits a second and logs a warning message when handling of events was cancelled by the server


# [15.1.0] - 2022-2-11 [PR: #132](https://github.com/dolittle/DotNET.SDK/pull/132)
## Summary

Adds two new event key selectors to projections, `StaticKey` and `KeyFromEventOccurred`

### Added

- `[StaticKey]` event key selector attribute for projection On-methods that sets a constant, static, key as the key of the read model
- `[KeyFromEventOccurred]` event key selector for projection On-methods that uses the event occurred metadata as the key for the projection read models formatted as the string given to the attribute. We currently support these formats:
    - yyyy-MM-dd
    - yyyy-MM
    - yyyy
    - HH:mm:ss
    - hh:mm:ss
    - HH:mm
    - hh:mm
    - HH
    - hh
    - yyyy-MM-dd HH:mm:ss
    - And the above in different orderings


# [15.0.1] - 2022-2-10 [PR: #131](https://github.com/dolittle/DotNET.SDK/pull/131)
## Summary

Do not throw exception when using dolittle middleware and it's not connected before a request is received, and fixes a configuration bug when getting a Dolittle Client directly from the service provider.

### Fixed

- In the `TenantScopedServiceProviderMiddleware` we check if the Dolittle Client is connected and log a message if it's not
- Using the `IServiceProvider.GetDolittleClient()` got the wrong configuration object.


# [15.0.0] - 2022-2-10 [PR: #130](https://github.com/dolittle/DotNET.SDK/pull/130)
## Summary

The Dolittle Client now fetches resources while establishing the initial connection so that we could make the resources interfaces synchronous, simplifying the usage and allowing us to more simply bind them in the DI container. The `.Connected` property on the client has been changed to a `Task` so you can await the connection asynchronously. The old boolean property has been moved to `.IsConnected`. 

### Added

- A new property `IDolittleClient.Connected` that returns a `Task` that is resolved when the client is successfully connected to a Runtime.

### Changed

- The `IMongoDBResource.GetDatabase()` returns an `IMongoDatabase` instead of a `Task<IMongoDatabase>` since the configuration is retrieved while connecting to the Runtime.
- The `IDolittleClient.Connected` boolean property has been renamed to `.IsConnected`.
- The `[MongoDBConvertTo(...)]` attribute was renamed to `[ConvertToMongoDB(...)]` as it was intended in the last release.

### Fixed

- Calling `IServiceProvider.GetDolittleClient()` could throw an exception if called while the client was in the process of connecting.


# [14.2.0] - 2022-2-9 [PR: #125](https://github.com/dolittle/DotNET.SDK/pull/125)
## Summary

Introduces APIs to configure secondary storage for Projection read models for querying, as introduced in https://github.com/dolittle/Runtime/pull/614 (requires Runtime v7.6.0). These changes makes it easy to query Projection read models by specifying that you want copies stored in MongoDB, and then use an `IMongoCollection<>` for that Projection as any other MongoDB collection. The Projection still operates normally and can be fetched from the Projection Store. Modifications of documents in the copied collections will affect the original Projection processing, but should be avoided as it could cause unexpected behaviour. The collections are automatically created and dropped as needed by the Runtime when Projections are created or changed.

There is currently no mechanism for detecting multiple projections copied to the same collection, so be aware of possible strange behaviour if you have multiple Projections with the same name.

### Added

- The `[CopyToMongoDB(...)]` attribute that enables read model copies for a Projection class to MongoDB. The default collection name is the same as the class name. The attribute accepts an argument to override the collection name.
- The `[ConvertToMongoDB(conversion)]` attribute to specify a BSON conversion to apply when copying the Projection read model to a MongoDB collection. By default the same conversions as the MongoDB driver uses is applied.
- A `.CopyToMongoDB(...)` method on the Projection builder for enabling read model copies for Projections created using the builder API. This method accepts a callback that you can use to set the collection name and conversions for the read model copies. You can also disable default MongoDB driver conversions.
- Binding for `IMongoDatabase` in the tenant scoped DI containers.
- Binding for `IMongoCollection<TReadModel>` in the tenant scoped DI containers for each Projection.
- Extension method `IMongoDatabase.GetCollection<TReadModel>(settings = null)` to get a collection using the name of the read model or the collection specified in the `[CopyToMongoDB(collection)]` attribute.

### Changed

- To make deserialising work from a Projection read model copy collection, we have enabled `IgnoreExtraElements` for all types in the MongoDB driver when the Dolittle Client is used. This is not default behaviour for the MongoDB driver, but when using MongoDB for read model storage, this should not affect the application adversely.


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

