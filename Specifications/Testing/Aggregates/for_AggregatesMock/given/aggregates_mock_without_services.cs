// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Aggregates;
using Machine.Specifications;

namespace Dolittle.SDK.Testing.Aggregates.for_AggregatesMock.given;

class aggregates_mock_without_services<TAggregate> : spec_for<TAggregate>
    where TAggregate : AggregateRoot
{
    Establish context = () => create_aggregates_mock = () => AggregatesMock.Create();
}