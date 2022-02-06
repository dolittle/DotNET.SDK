// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Sample code for the tutorial at https://dolittle.io/tutorials/getting-started/projections/

using System;
using Dolittle.SDK.Projections;
using Dolittle.SDK.Projections.Copies.MongoDB;

[Projection("98f9db66-b6ca-4e5f-9fc3-638626c9ecfa")]
[CopyProjectionToMongoDB]
public class DishCounter
{
    public int NumberOfTimesPrepared = 0;
    [MongoDBConvertTo(Conversion.None)]
    public string Name = "";

    [KeyFromProperty("Dish")]
    public void On(DishPrepared @event, ProjectionContext context)
    {
        Name = @event.Dish; 
        NumberOfTimesPrepared ++;
    }
}
