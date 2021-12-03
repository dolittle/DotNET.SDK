using Dolittle.SDK.Events;

namespace Recipes;
 
[EventType("1a8d322e-edd8-48aa-a4ed-93a020facbbf")]
public record IngredientAmountChangedForRecipe(string Ingredient, int NewAmountNeeded, int OldAmountNeeded);