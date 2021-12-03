using Dolittle.SDK.Events;

namespace Restaurant.Menus;

[EventType("eb49c188-22ff-4901-82be-838e2c98cfd6")]
public record DishAddedToMenu(string Dish);