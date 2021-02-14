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

