// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Testing.Projections;
using FluentAssertions;
using Kitchen;

namespace ASP.NET.Tests;

public class DishesPreparedByKitchenTests : ProjectionTests<DishesPreparedByKitchen>
{
    const string KitchenId = "1";

    [Fact]
    public void WhenSingleDishPrepared()
    {
        var dish = "Some dish";
        var chef = "Some chef";
        WhenAggregateMutated<Kitchen.Kitchen>(KitchenId, it => it.PrepareDish(chef, dish));
        
        AssertThat.HasReadModel(KitchenId)
            .AndThat(it =>
            {
                it.DishesPrepared.Should().ContainKey(dish);
                it.DishesPrepared[dish].Should().Be(1);
            });
    }
    
    [Fact]
    public void WhenMultipleDishesPrepared()
    {
        var dish = "Pizza";
        var dish2 = "Burgers";
        var chef = "Some chef";
        WhenAggregateMutated<Kitchen.Kitchen>(KitchenId, it =>
        {
            it.Restock(8, "SomeSupplier");
            it.PrepareDish(chef, dish);
            it.PrepareDish(chef, dish2);
            it.PrepareDish(chef, dish2);
            
        });
        
        AssertThat.HasReadModel(KitchenId)
            .AndThat(it =>
            {
                it.DishesPrepared.Should().ContainKey(dish);
                it.DishesPrepared[dish].Should().Be(1);
                
                it.DishesPrepared.Should().ContainKey(dish2);
                it.DishesPrepared[dish2].Should().Be(2);
            });
    }
}