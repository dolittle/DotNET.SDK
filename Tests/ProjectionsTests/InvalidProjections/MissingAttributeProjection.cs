// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Projections.Types;

namespace Dolittle.SDK.Projections.InvalidProjections;

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
