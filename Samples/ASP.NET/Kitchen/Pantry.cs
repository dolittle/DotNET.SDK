// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Projections;

namespace Kitchen;

[Projection("db4b5141-b3f1-4bea-8eb5-9eadd5338df8", idleUnloadTimeout: "00:00:03", queryInMemory: true)]
public class Pantry : ReadModel, ICloneable
{
    public int RemainingIngredients { get; set; } = 100;

    public void On(DishPrepared @event)
    {
        RemainingIngredients--;
    }

    public object Clone() => MemberwiseClone();
}