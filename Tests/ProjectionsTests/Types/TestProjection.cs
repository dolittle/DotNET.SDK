// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events;
using Dolittle.SDK.Projections;

namespace ProjectionsTests;

[Projection("055319f1-6af8-48a0-b190-323e21ba6cde")]
public class TestProjection : ProjectionBase
{
    public int UpdateCount { get; set; }
    public string Content { get; set; } = string.Empty;
    public int TheNumber { get; set; }

    void On(SomeEvent evt, ProjectionContext ctx)
    {
        UpdateCount++;
        Content = evt.Thing;
    }

    void On(SomeOtherEvent evt, EventContext ctx)
    {
        UpdateCount++;
        TheNumber = evt.SomeNumber;
    }

    ProjectionResult<TestProjection> On(DeleteEvent _)
    {
        return ProjectionResult<TestProjection>.Delete;
    }
}
