// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Sample code for the tutorial at https://dolittle.io/tutorials/embeddings/

using Dolittle.SDK.Events;

namespace Kitchen
{
    [EventType("2ffc6df2-9c83-4906-a2b3-e3c06a485476")]
    public class DishRemoved
    {
        public DishRemoved(string dish)
        {
            Dish = dish;
        }

        public string Dish { get; }
    }
}
