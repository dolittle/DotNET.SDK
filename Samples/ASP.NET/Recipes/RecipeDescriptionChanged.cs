using Dolittle.SDK.Events;

namespace Recipes;

[EventType("71a2028e-7f51-40f2-be76-e00733b62e73")]
public record RecipeDescriptionChanged(string Description);