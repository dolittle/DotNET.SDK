using Dolittle.SDK.Events;

[EventType("8b376dd2-102a-440b-81c5-1526353ab4ab")]
public record IngredientUsed(string Ingredient, int Amount, int Stock);