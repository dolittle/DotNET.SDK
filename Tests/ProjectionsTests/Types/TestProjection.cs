﻿// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.SDK.Projections.Types;

[Projection("055319f1-6af8-48a0-b190-323e21ba6cde", idleUnloadTimeout: "00:01:00")]
public class TestProjection : ReadModel
{
    public int UpdateCount { get; set; }
    public string Content { get; set; } = string.Empty;
    public int TheNumber { get; set; }

    public void On(SomeEvent evt, ProjectionContext ctx)
    {
        UpdateCount++;
        Content = evt.Thing;
    }

    public void On(SomeOtherEvent evt, ProjectionContext ctx)
    {
        UpdateCount++;
        TheNumber = evt.SomeNumber;
    }

    public ProjectionResult<TestProjection> On(DeleteEvent _, ProjectionContext ctx)
    {
        return ProjectionResult<TestProjection>.Delete;
    }
}
