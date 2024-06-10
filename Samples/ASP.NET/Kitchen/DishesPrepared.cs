// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.SDK.Projections;

namespace Kitchen;

[Projection("65bb96a8-b4e0-45dd-a1b3-89c3dbbbb173")]
public class DishesPreparedByKitchen : ReadModel
{
    private readonly Dictionary<string, int> _dishesPrepared = new();
    private int _totalDishesPrepared;

    public IReadOnlyDictionary<string, int> DishesPrepared => _dishesPrepared;
    public int TotalDishesPrepared => _totalDishesPrepared;

    public void On(DishPrepared evt, ProjectionContext context)
    {
        _totalDishesPrepared++;
        if (!_dishesPrepared.TryAdd(evt.Dish, 1))
        {
            _dishesPrepared[evt.Dish]++;
        }
    }
}