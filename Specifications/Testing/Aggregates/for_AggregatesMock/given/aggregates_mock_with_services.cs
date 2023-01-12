// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Aggregates;
using Machine.Specifications;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.SDK.Testing.Aggregates.for_AggregatesMock.given;

class aggregates_mock_with_services<TAggregate> : spec_for<TAggregate>
    where TAggregate : AggregateRoot
{
    protected static Action<IServiceCollection> configure_services;
    Establish context = () =>
    {
        configure_services = null;
        create_aggregates_mock = () => AggregatesMock.Create(configure_services);
    };
}