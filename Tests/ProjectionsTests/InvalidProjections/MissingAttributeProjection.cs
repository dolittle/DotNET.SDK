// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events;
using Dolittle.SDK.Projections;
using ProjectionsTests;

namespace Dolittle.SDK.ProjectionTests.InvalidProjections;

/// <summary>
/// Projection without the [Projection] attribute.
/// </summary>
public class MissingAttributeProjection : ReadModel
{
    public string Content { get; set; } = string.Empty;

    public void On(SomeEvent evt, ProjectionContext ctx)
    {
        Content = evt.Thing;
    }
}
