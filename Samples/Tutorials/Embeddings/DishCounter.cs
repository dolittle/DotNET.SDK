// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Sample code for the tutorial at https://dolittle.io/tutorials/embeddings/

using Dolittle.SDK.Events;
using Dolittle.SDK.Embeddings;
using Dolittle.SDK.Projections;

namespace Kitchen
{
    [Embedding("205fb7c7-40f8-4d30-8981-b97209a3f70e")]
    public class DishCounter
    {
        public int NumberOfTimesPrepared = 0;
        public string Dish = "";

        public object Compare(DishCounter receivedState, EmbeddingContext context)
        {
            if (receivedState.NumberOfTimesPrepared > NumberOfTimesPrepared)
            {
                return new DishPrepared(receivedState.Dish);
            }
            return null;
        }

        public object Remove(EmbeddingContext context)
        {
            return new DishRemoved(context.Key);
        }

        public void On(DishPrepared @event, EmbeddingProjectContext context)
        {
            NumberOfTimesPrepared++;
            Dish = @event.Dish;
        }

        public ProjectionResultType On(DishRemoved @event, EmbeddingProjectContext context)
        {
            return ProjectionResultType.Delete;
        }
    }
}
