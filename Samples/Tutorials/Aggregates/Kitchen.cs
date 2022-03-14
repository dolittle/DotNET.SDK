// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Sample code for the tutorial at https://dolittle.io/tutorials/getting-started/csharp/

using System;
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;

[AggregateRoot("01ad9a9f-711f-47a8-8549-43320f782a1e")]
public class Kitchen : AggregateRoot
{
    int _ingredients = 2;

    public Kitchen(EventSourceId eventSource)
        : base(eventSource)
    {
    }

    public void PrepareDish(string dish, string chef)
    {
        if (_ingredients <= 0) throw new Exception("We have run out of ingredients, sorry!");
        Apply(new DishPrepared(dish, chef));
        Console.WriteLine($"Kitchen {EventSourceId} prepared a {dish}, there are {_ingredients} ingredients left.");
    }

    void On(DishPrepared @event)
        => _ingredients--;
}

