// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Sample code for the tutorial at https://dolittle.io/tutorials/getting-started/projections/

using System;
using Dolittle.SDK.Projections;
using Dolittle.SDK.Projections.Copies.MongoDB;

[Projection("98f9db66-b6ca-4e5f-9fc3-638626c9ecfa")]
public class DishCounter: ReadModel
{
    public int NumberOfTimesPrepared = 0;
    public string Name = "";
    public DateTimeOffset LastPrepared = DateTimeOffset.MinValue;

    [KeyFromProperty("Dish")]
    public void On(DishPrepared @event, ProjectionContext context)
    {
        Name = @event.Dish; 
        NumberOfTimesPrepared ++;
        LastPrepared = context.EventContext.Occurred;
    }
}
