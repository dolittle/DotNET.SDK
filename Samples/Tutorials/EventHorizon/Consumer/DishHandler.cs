// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Sample code for the tutorial at https://dolittle.io/tutorials/getting-started/csharp/

using System;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Handling;

[EventHandler("f2d366cf-c00a-4479-acc4-851e04b6fbba")]
public class DishHandler
{
    public void Handle(DishPrepared @event, EventContext eventContext)
    {
        Console.WriteLine($"{@event.Chef} has prepared {@event.Dish}. Yummm!");
    }
}

