// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Sample code for the tutorial at https://dolittle.io/tutorials/getting-started/csharp/

using Dolittle.SDK.Events;

[EventType("1844473f-d714-4327-8b7f-5b3c2bdfc26a")]
public class DishPrepared
{
    public DishPrepared(string dish, string chef)
    {
        Dish = dish;
        Chef = chef;
    }

    public string Dish { get; }
    public string Chef { get; }
}
