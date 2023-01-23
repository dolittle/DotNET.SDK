using System;
using Machine.Specifications;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.SDK.Testing.Aggregates.for_AggregateMock.given;

class an_aggregate_of_aggregate_with_dependencies
{
    protected static Action<IServiceCollection> configure_services;
    protected static AggregateOfMock<AggregateRootWithDependencies> aggregate_of => _aggregate_of ??= AggregateOfMock<AggregateRootWithDependencies>.Create(configure_services);
    static AggregateOfMock<AggregateRootWithDependencies> _aggregate_of;
    
    Establish context = () =>
    {
        configure_services = null;
    };
}