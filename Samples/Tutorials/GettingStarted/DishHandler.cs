// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Sample code for the tutorial at https://dolittle.io/tutorials/getting-started/csharp/

using System;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Handling;
using Microsoft.Extensions.Logging;

[EventHandler("f2d366cf-c00a-4479-acc4-851e04b6fbba")]
public class DishHandler
{
    readonly ILogger _logger;

    public DishHandler(ILogger<DishHandler> logger)
    {
        _logger = logger;
    }

    public void Handle(DishPrepared @event, EventContext eventContext)
    {
        _logger.LogInformation("{Chef} has prepared {Dish}. Yummm!", @event.Chef, @event.Dish);
    }
}
