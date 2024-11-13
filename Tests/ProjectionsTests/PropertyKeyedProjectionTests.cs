// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events;
using Dolittle.SDK.Testing.Projections;
using FluentAssertions;
using MongoDB.Bson.Serialization.Attributes;
using Xunit;

namespace Dolittle.SDK.Projections;

[EventType("215ecbf9-cfe8-4656-84c5-5f6302ef6cd3")]
public record AnEvent(string TheProperty, string TheValue);

[EventType("7c85e7c3-aff4-42f4-b577-d70000b636c6")]
public record ADeleteEvent(string TheProperty);

[Projection("f850fb88-2f09-45ca-a987-94b7fbb115a5")]
public class PropertyKeyedProjection : ReadModel
{
    public string TheValue { get; set; }

    [KeyFromProperty(nameof(AnEvent.TheProperty))]
    public void On(AnEvent evt)
    {
        TheValue = evt.TheValue;
    }

    [KeyFromProperty(nameof(AnEvent.TheProperty))]
    public ProjectionResultType On(ADeleteEvent evt) => ProjectionResultType.Delete;
}

public class PropertyKeyedProjectionTests : ProjectionTests<PropertyKeyedProjection>
{
    const string EventSource = "Foo";
    const string Property = "TheProperty";

    [Fact]
    public void CanProjectOnProperty()
    {
        WithEvent(EventSource, new AnEvent(Property, "42"));

        AssertThat.ReadModel(Property).TheValue.Should().Be("42");
    }

    [Fact]
    public void CanProjectDeleteOnProperty()
    {
        WithEvent(EventSource, new AnEvent(Property, "42"));
        WithEvent(EventSource, new ADeleteEvent(Property));

        AssertThat.ReadModelDoesNotExist(Property);
    }
}
