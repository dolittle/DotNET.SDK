// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Dolittle.SDK.Projections;

namespace Customers;

[Projection("185107c2-f897-40c8-bb06-643b3642f230")]
public class DishesEaten: ProjectionBase
{
    public string[] Dishes { get; set; } = {};
    
    [KeyFromEventSource]
    public void On(DishEaten evt, ProjectionContext ctx)
    {
        Dishes = Dishes.Append(evt.Dish).ToArray();
    }
}
