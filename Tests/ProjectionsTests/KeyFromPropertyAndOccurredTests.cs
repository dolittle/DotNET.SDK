// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Globalization;
using Dolittle.SDK.Projections.Occurred;
using Dolittle.SDK.Testing.Projections;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace Dolittle.SDK.Projections;

public class KeyFromPropertyAndOccurredTests : ProjectionTests<SalesPerDayByStoreProperty>
{
    [Fact]
    public void ShouldAggregateByDayAndStore()
    {
        WithEvent(Guid.NewGuid().ToString(), new ProductSold("store1", "product1", 10, 100),
            DateTimeOffset.Parse("2024-01-01", NumberFormatInfo.InvariantInfo));
        WithEvent(Guid.NewGuid().ToString(), new ProductSold("store1", "product2", 10, 100),
            DateTimeOffset.Parse("2024-01-01", NumberFormatInfo.InvariantInfo));
        WithEvent(Guid.NewGuid().ToString(), new ProductSold("store1", "product1", 10, 10), DateTimeOffset.Parse("2024-01-02", NumberFormatInfo.InvariantInfo));
        WithEvent(Guid.NewGuid().ToString(), new ProductSold("store2", "product1", 5, 10), DateTimeOffset.Parse("2024-01-01", NumberFormatInfo.InvariantInfo));

        using var _ = new AssertionScope();
        
        AssertThat.HasReadModel("store1_2024-01-01").Where(
            it => it.TotalSales.Should().Be(2000),
            it => it.Store.Should().Be("store1"),
            it => it.Date.Should().Be(new DateOnly(2024, 1, 1))
        );
        
        AssertThat.HasReadModel("store1_2024-01-02").Where(
            it => it.TotalSales.Should().Be(100),
            it => it.Store.Should().Be("store1"),
            it => it.Date.Should().Be(new DateOnly(2024, 1, 2))
        );
        
        AssertThat.HasReadModel("store2_2024-01-01").Where(
            it => it.TotalSales.Should().Be(50),
            it => it.Store.Should().Be("store2"),
            it => it.Date.Should().Be(new DateOnly(2024, 1, 1))
        );
    }
}
