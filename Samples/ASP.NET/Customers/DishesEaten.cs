// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Dolittle.SDK.Projections;
using Dolittle.SDK.Projections.Copies.MongoDB;

namespace Customers;

[Projection("185107c2-f897-40c8-bb06-643b3642f229")]
[CopyProjectionToMongoDB("dishes_eaten")]
public class DishesEaten
{
    public string[] Dishes { get; set; } = {};
    
    [KeyFromEventSource]
    public void On(DishEaten evt, ProjectionContext ctx)
    {
        Dishes = Dishes.Append(evt.Dish).ToArray();
    }
}
