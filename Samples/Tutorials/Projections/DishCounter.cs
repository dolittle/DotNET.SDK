// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Sample code for the tutorial at https://dolittle.io/tutorials/getting-started/projections/

using Dolittle.SDK.Projections;

[Projection("98f9db66-b6ca-4e5f-9fc3-638626c9ecfa")]
public class DishCounter
{
    public int NumberOfTimesPrepared = 0;

    [KeyFromProperty("Dish")]
    public void On(DishPrepared @event, ProjectionContext context)
    {
        NumberOfTimesPrepared ++;
    }
}
