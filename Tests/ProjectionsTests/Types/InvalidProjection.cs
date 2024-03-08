// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events;
using Dolittle.SDK.Projections;

namespace ProjectionsTests;

/// <summary>
/// Projection with multiple on-methods for the same event type.
/// </summary>
[Projection("055319f1-6af8-48a0-b190-323e21ba6cde")]
public class InvalidProjection : ReadModel
{
    public string Content { get; set; } = string.Empty;

    void On(SomeEvent evt, ProjectionContext ctx)
    {
        Content = evt.Thing;
    }

    void On(SomeEvent evt, EventContext ctx)
    {
        Content = evt.Thing;
    }
}
