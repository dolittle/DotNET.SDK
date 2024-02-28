// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.SDK.Projections.Internal;

static class Extensions
{
    public static ProjectionResult<TProjection> Apply<TProjection>(this TProjection result, object evt, ProjectionContext context)
        where TProjection : ProjectionBase, new() =>
        ProjectionType<TProjection>.Apply(result, evt, context);
}
