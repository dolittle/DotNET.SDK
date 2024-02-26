// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Projections.Store;

/// <summary>
/// Exception that gets thrown when attempting to associate multiple instance of <see cref="Type"/> with a single projection.
/// </summary>
public class CannotAssociateMultipleTypesWithProjection : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CannotAssociateMultipleTypesWithProjection"/> class.
    /// </summary>
    /// <param name="projection">The <see cref="ProjectionId"/> that was attempted to associate with a <see cref="Type"/>.</param>
    /// <param name="type">The <see cref="Type"/> that was attempted to associate with.</param>
    /// <param name="existing">The <see cref="Type"/> that the <see cref="ProjectionId"/> was already associated with.</param>
    public CannotAssociateMultipleTypesWithProjection(ProjectionId projection, Type type, Type existing)
        : base($"{projection} cannot be associated with {type} because it is already associated with {existing}")
    {
    }
}
