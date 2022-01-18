// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Projections.Store;

/// <summary>
/// Exception that gets thrown when attempting to associate multiple instance of projection with a single <see cref="Type"/>.
/// </summary>
public class CannotAssociateMultipleProjectionsWithType : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CannotAssociateMultipleProjectionsWithType"/> class.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> that was attempted to associate with a <see cref="ProjectionId"/>.</param>
    /// <param name="projection">The <see cref="ProjectionId"/> that was attempted to associate with.</param>
    /// <param name="existing">The <see cref="ProjectionId"/> that the <see cref="Type"/> was already associated with.</param>
    public CannotAssociateMultipleProjectionsWithType(Type type, ProjectionId projection, ProjectionId existing)
        : base($"{type} cannot be associated with {projection} because it is already associated with {existing}")
    {
    }
}