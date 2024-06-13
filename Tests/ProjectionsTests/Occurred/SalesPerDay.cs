// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events;

namespace Dolittle.SDK.Projections.Occurred;

[EventType("37c19042-3ac8-4ec0-8de1-9ecb4b8c56b0")]
public record ProductSold(string Store, string Product, decimal Quantity, decimal Price);

[Projection("3dce944f-279e-4150-bef3-dd9e113220c6")]
public class SalesPerDayByStoreProperty : ReadModel
{
    public string Store { get; private set; }
    public DateOnly Date { get; private set; }

    public decimal TotalSales { get; private set; }

    [KeyFromPropertyAndOccurred(nameof(ProductSold.Store), "yyyy-MM-dd")]
    public void On(ProductSold evt, EventContext ctx)
    {
        if (string.IsNullOrEmpty(Store))
        {
            Store = evt.Store;
            Date = new DateOnly(ctx.Occurred.Year, ctx.Occurred.Month, ctx.Occurred.Day);
        }

        TotalSales += evt.Quantity * evt.Price;
    }
}

[Projection("3dce944f-279e-4150-bef3-dd9e113220c6")]
public class SalesPerDayTotalByEventSource : ReadModel
{
    public string Store { get; private set; } = null!;
    public DateOnly Date { get; private set; }

    public decimal TotalSales { get; private set; }

    [KeyFromEventSourceAndOccurred("yyyy-MM-dd")]
    public void On(ProductSold evt, EventContext ctx)
    {
        if (string.IsNullOrEmpty(Store))
        {
            Store = evt.Store;
            Date = new DateOnly(ctx.Occurred.Year, ctx.Occurred.Month, ctx.Occurred.Day);
        }

        TotalSales += evt.Quantity * evt.Price;
    }
}

class ProductKeySelector: IKeySelector<ProductSold>
{
    public Key Selector(ProductSold evt, EventContext ctx) => $"{evt.Store}_{ctx.Occurred.Year}-{ctx.Occurred.Month:D2}-{ctx.Occurred.Day:D2}";
}

[Projection("3dce944f-279e-4150-bef3-dd9e113220c6")]
public class SalesPerDayTotalByFunction : ReadModel
{
    public string Store { get; private set; } = null!;
    public DateOnly Date { get; private set; }

    public decimal TotalSales { get; private set; }

    [KeyFromFunction<ProductKeySelector, ProductSold>]
    public void On(ProductSold evt, EventContext ctx)
    {
        if (string.IsNullOrEmpty(Store))
        {
            Store = evt.Store;
            Date = new DateOnly(ctx.Occurred.Year, ctx.Occurred.Month, ctx.Occurred.Day);
        }

        TotalSales += evt.Quantity * evt.Price;
    }
    
    public static string GetKey(ProductSold evt, EventContext ctx) => $"{evt.Store}_{ctx.Occurred.Year}-{ctx.Occurred.Month:D2}-{ctx.Occurred.Day:D2}";
}

