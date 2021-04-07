# [8.4.0-projections.0] - 2021-4-7 [PR: #55](https://github.com/dolittle/DotNET.SDK/pull/55)
## Summary

Adds projections

### Added

- Support for building and registering projections


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

