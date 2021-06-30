// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Sample code for the tutorial at https://dolittle.io/tutorials/getting-started/csharp/

using Dolittle.SDK.Events;

namespace Kitchen
{
    [Embedding("205fb7c7-40f8-4d30-8981-b97209a3f70e")]
    public class DishCounter
    {

        public int NumberOfTimesPrepared = 0;
        public string Dish = "";

        public object[] Compare(DishCounter receivedState, EmbeddingContext context)
        {
            if (receivedState.NumberOfTimesPrepared > NumberOfTimesPrepared)
            {
                return new DishPrepared(receivedState.Dish);
            }
        }

        public void On(DishPrepared @event, EmbeddingProjectContext context)
        {
            NumberOfTimesPrepared++;
            Dish = @event.Dish;
        }

        public EmbeddingResultType On(DishRemoved @event, EmbeddingProjectContext context)
        {
            return EmbeddingResultType.Delete;
        }
    }
}
