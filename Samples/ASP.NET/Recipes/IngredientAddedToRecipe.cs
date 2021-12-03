using Dolittle.SDK.Events;

namespace Recipes;
 
[EventType("a695803e-9644-4a25-8f0b-e9a7ebf38cd1")]
public record IngredientAddedToRecipe(string Ingredient, int AmountNeeded);