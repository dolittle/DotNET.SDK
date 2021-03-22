// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Sample code for the tutorial at https://dolittle.io/tutorials/getting-started/csharp/

using System;
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;

namespace Kitchen
{
    [AggregateRoot("01ad9a9f-711f-47a8-8549-43320f782a1e")]
    public class Kitchen : AggregateRoot
    {
        int _counter;

        public Kitchen(EventSourceId eventSource)
            : base(eventSource)
        {
        }

        public void PrepareDish(string dish, string chef)
        {
            Apply(new DishPrepared(dish, chef));
            Console.WriteLine($"Kitchen Aggregate {EventSourceId} has applied {_counter} {typeof(DishPrepared)} events");
        }

        void On(DishPrepared @event)
            => _counter++;
    }
}
