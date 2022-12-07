// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Sample code for the tutorial at https://dolittle.io/tutorials/getting-started/csharp/

using System;
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;
using Microsoft.Extensions.Logging;

[AggregateRoot("01ad9a9f-711f-47a8-8549-43320f782a1e")]
public class Kitchen : AggregateRoot
{
    readonly EventSourceId _eventSource;
    readonly ILogger<Kitchen> _logger;
    int _ingredients = 2;

    public Kitchen(EventSourceId eventSource, ILogger<Kitchen> logger)
    {
        _eventSource = eventSource;
        _logger = logger;
    }

    public void PrepareDish(string dish, string chef)
    {
        if (_ingredients <= 0)
        {
            throw new Exception("We have run out of ingredients, sorry!");
        }
        Apply(new DishPrepared(dish, chef));
        _logger.LogInformation("Kitchen {EventSourceId} prepared a {Dish}, there are {Ingredients} ingredients left", _eventSource, dish, _ingredients);
    }

    void On(DishPrepared @event)
        => _ingredients--;
}

