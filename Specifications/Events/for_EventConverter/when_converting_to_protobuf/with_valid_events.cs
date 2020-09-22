// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Machine.Specifications;
using Contracts = Dolittle.Runtime.Events.Contracts;

namespace Dolittle.SDK.Events.for_EventConverter.when_converting_to_protobuf
{
    public class with_valid_events : given.events
    {
        static IEnumerable<Contracts.UncommittedEvent> result;
        static int count;

        Because of = () =>
        {
            result = converter.ToProtobuf(uncommitted_events);
            foreach (var @event in result)
            {
                ++count;
            }

            Console.WriteLine(result.GetType());
        };

        It should_have_protobuf_uncommitted_events = () => result.ShouldBeAssignableTo<IEnumerable<Contracts.UncommittedEvent>>();
        It should_have_processed_all_events = () => count.ShouldEqual(uncommitted_events.Count);
    }
}
