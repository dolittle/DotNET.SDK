// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Sample code for the tutorial at https://dolittle.io/tutorials/getting-started/csharp/

using Dolittle.SDK.Events;

namespace Kitchen 
{
    [EventType("551bbdc1-6ef2-4a9d-9a2e-91b380632b59")]
    public class DishCreated
    {
        public DishCreated (string dish, string chef)
        {
            Dish = dish;
            Chef = chef;
        }

        public string Dish { get; }
        public string Chef { get; }
    }
}
