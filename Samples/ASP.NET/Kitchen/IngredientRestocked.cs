// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events;

// namespace Kitchen;

[EventType("34b2445e-431f-4add-bfb8-7b65a54a5e9d")]
public record IngredientRestocked(string Ingredient, int Amount, int Stock);

[EventType("34b2445e-431f-4add-bfb8-7b65a54a5e9d")]
public record IngredientUsed(string Ingredient, int Amount, int Stock);

