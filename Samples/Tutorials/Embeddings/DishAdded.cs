// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Sample code for the tutorial at https://dolittle.io/tutorials/embeddings/

using Dolittle.SDK.Events;

namespace Kitchen
{
    [EventType("8320ed59-c1d6-4c62-96b1-41117a6c42e0")]
    public class DishAdded
    {
        public DishAdded(string dish)
        {
            Dish = dish;
        }

        public string Dish { get; }
    }
}
