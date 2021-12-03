using System.Collections.Generic;
using Dolittle.SDK.Projections;

[Projection("26b33f54-fc4e-4c39-8e60-8354134911a7")]
public class RestaurantMenu
{
    public Dictionary<string, string> Dishes = new();
    
    [KeyFromEventSource]
    public void On(DishAddedToMenu @event)
        => Dishes[@event.Dish] = "";

    [KeyFromEventSource]
    public void On(DishRemovedFromMenu @event)
        => Dishes.Remove(@event.Dish);
}