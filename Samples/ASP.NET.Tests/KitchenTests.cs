using Dolittle.SDK.Testing.Aggregates;
using FluentAssertions;
using Kitchen;

namespace ASP.NET.Tests;

public class KitchenTests : AggregateRootTests<Kitchen.Kitchen>
{
    private const string Id = "SomeKitchenId";
    private const string Chef = "SomeChef";
    private const string Dish = "Pizza";

    public KitchenTests() : base(Id)
    {
    }

    [Fact]
    public void WhenPreparingDish()
    {
        WhenPerforming(it => it.PrepareDish(Chef, Dish));

        AssertThat.ShouldHaveSingleEvent<DishPrepared>().AndThat(evt =>
        {
            evt.Chef.Should().Be(Chef);
            evt.Dish.Should().Be(Dish);
        });
    }

    [Fact]
    public void WhenPreparingDishWithoutIngredients()
    {
        WithAggregateInState(it =>
        {
            it.PrepareDish(Chef, Dish);
            it.PrepareDish(Chef, Dish);
        });

        VerifyThrowsExactly<OutOfIngredients>(it => it.PrepareDish(Chef, Dish));
    }
    
    [Fact]
    public void WhenPreparingAfterRestock()
    {
        WithAggregateInState(it =>
        {
            it.PrepareDish(Chef, Dish);
            it.PrepareDish(Chef, Dish);
            it.Restock(10, "SomeSupplier");
        });

        WhenPerforming(it => it.PrepareDish(Chef, Dish));

        AssertThat.ShouldHaveSingleEvent<DishPrepared>().AndThat(evt =>
        {
            evt.Chef.Should().Be(Chef);
            evt.Dish.Should().Be(Dish);
        });
    }

    [Fact]
    public void WhenRestocking()
    {
        WhenPerforming(it => it.Restock(5, "SomeSupplier"));

        AssertThat.ShouldHaveSingleEvent<Restocked>().AndThat(evt =>
        {
            evt.Amount.Should().Be(5);
            evt.Supplier.Should().Be("SomeSupplier");
        });
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public void WhenRestockingWithInvalidAmount(int amount)
    {
        var exception = VerifyThrowsExactly<ArgumentException>(it => it.Restock(amount, "SomeSupplier"));
        
        exception.ParamName.Should().Be("amount");
    }
    
    [Theory]
    [InlineData(null, null)]
    [InlineData("", "")]
    [InlineData("", "ADish")]
    [InlineData("Chefo", "")]
    public void WhenPreparingDishWithInvalidArguments(string? chef, string? dish)
    {
        VerifyThrows(it => it.PrepareDish(chef, dish));
    }
}