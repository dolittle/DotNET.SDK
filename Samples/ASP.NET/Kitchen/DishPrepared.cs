// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events;

namespace Kitchen;

[EventType("1844473f-d714-4327-8b7f-5b3c2bdfc26a")]
public record DishPrepared(string Dish, string Chef);
