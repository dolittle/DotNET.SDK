// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Sample code for the tutorial at https://dolittle.io/tutorials/getting-started/projections/

namespace Kitchen
{
    [Projection("0405b93f-1461-472c-bdc2-f89e0afd4dfe")]
    public class Menu
    {
        public List<string> Dishes;

        [KeyFromEventSource]
        [KeyFromProperty("Chef")]
        public void On(DishPrepared @event, EventContext eventContext)
        {
            if (!Dishes.Contains(@event.Dish)
            {
                Dishes.Add(@event.Dish);
            }
        }
    }
}
