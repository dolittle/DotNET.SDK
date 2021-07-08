// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Sample code for the tutorial at https://dolittle.io/tutorials/embeddings/

using System;
using Dolittle.SDK.Embeddings;
using Dolittle.SDK.Projections;

namespace Kitchen
{
    [Embedding("205fb7c7-40f8-4d30-8981-b97209a3f70e")]
    public class DishCounter
    {
        public int NumberOfTimesPrepared { get; set; } = 0;
        public string Dish { get; set; } = "";

        public object ResolveUpdateToEvents(DishCounter updatedState, EmbeddingContext context)
        {
            if (updatedState.Dish != Dish)
            {
                return new DishAdded(updatedState.Dish);
            }
            else if (updatedState.NumberOfTimesPrepared > NumberOfTimesPrepared)
            {
                return new DishPrepared(updatedState.Dish);
            }

            throw new NotImplementedException();
        }

        public object ResolveDeletionToEvents(EmbeddingContext context)
        {
            return new DishRemoved(context.Key);
        }

        public void On(DishAdded @event, EmbeddingProjectContext context)
        {
            Dish = @event.Dish;
        }
        public void On(DishPrepared @event, EmbeddingProjectContext context)
        {
            NumberOfTimesPrepared++;
        }

        public ProjectionResultType On(DishRemoved @event, EmbeddingProjectContext context)
        {
            return ProjectionResultType.Delete;
        }
    }
}
