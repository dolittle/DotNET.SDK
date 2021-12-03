using Dolittle.SDK.Events;

namespace Restaurant.Menus;

[EventType("8a3886cb-1571-4200-8166-6aa7815b81f0")]
public record DishRemovedFromMenu(string Dish);