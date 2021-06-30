// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Sample code for the tutorial at https://dolittle.io/tutorials/getting-started/csharp/

using Dolittle.SDK.Events;

namespace Kitchen
{
    [EventType("204b57bc-dc01-417c-92fb-047d7a2f2697")]
    public class DishPrepared
    {
        public DishPrepared(string dish)
        {
            Dish = dish;
        }

        public string Dish { get; }
    }
}
