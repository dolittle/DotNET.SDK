using System.Globalization;
using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Common.Model;
using Dolittle.SDK.Events.Handling.Builder;
using Dolittle.SDK.Events.Handling.Builder.Convention.Type;
using Events.Handling;
using FluentAssertions;

namespace Dolittle.SDK.Events.Handling;

public class DynamicProcessRangeHandlerTests
{
    [Theory]
    [InlineData(ProcessFrom.Earliest, "2025-01-06T15:19:56", "2025-01-07T15:19:56")]
    [InlineData(ProcessFrom.Latest, null, null)]
    public void WhenRangeIsDynamicallySet(ProcessFrom mode, string? fromStr, string? toStr)
    {
        DateTimeOffset? from = fromStr is null ? null : DateTimeOffset.Parse(fromStr, CultureInfo.InvariantCulture);
        DateTimeOffset? to = toStr is null ? null : DateTimeOffset.Parse(toStr, CultureInfo.InvariantCulture);

        TestSelector.ProcessFrom = mode;
        TestSelector.From = from;
        TestSelector.To = to;


        var modelBuilder = new ModelBuilder();
        var clientBuildResults = new ClientBuildResults();
        var builder = new EventHandlersBuilder(modelBuilder, clientBuildResults);
        builder.RegisterAllFrom(typeof(DynamicProcessRangeHandler).Assembly);

        var model = modelBuilder.Build(clientBuildResults);

        clientBuildResults.Failed.Should().BeFalse();
        model.Should().NotBeNull();

        var dynamicBinding = model.GetProcessorBuilderBindings<ConventionTypeEventHandlerBuilder>()
            .Select(it => it.Identifier)
            .OfType<EventHandlerModelId>()
            .Single(it => it.Id.ToString().Equals(DynamicProcessRangeHandler.Identifier));

        dynamicBinding.ResetTo.Should().Be(mode);
        dynamicBinding.StartFrom.Should().Be(from);
        dynamicBinding.StopAt.Should().Be(to);
    }
}
