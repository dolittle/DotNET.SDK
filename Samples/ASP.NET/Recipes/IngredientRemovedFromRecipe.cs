using Dolittle.SDK.Events;

namespace Recipes;
 
[EventType("e0a76e98-b82a-43f6-890b-4f9873c3f7ad")]
public record IngredientRemovedFromRecipe(string Ingredient);