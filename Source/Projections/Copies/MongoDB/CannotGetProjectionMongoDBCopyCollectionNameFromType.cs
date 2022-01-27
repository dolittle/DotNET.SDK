// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Projections.Copies.MongoDB;

/// <summary>
/// Exception that gets thrown when the <see cref="ProjectionMongoDBCopyCollectionName"/> cannot be retrieved from a projection read model <see cref="Type"/>.
/// </summary>
public class CannotGetProjectionMongoDBCopyCollectionNameFromType : Exception
{
    /// <summary>
    /// Initializes a new instance of thee <see cref="CannotGetProjectionMongoDBCopyCollectionNameFromType"/> class.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> that the <see cref="ProjectionMongoDBCopyCollectionName"/> cannot be retrieved from.</param>
    /// <param name="exception">The optional inner exception.</param>
    public CannotGetProjectionMongoDBCopyCollectionNameFromType(Type type, Exception exception = default)
        : base($"Cannot get the {nameof(ProjectionMongoDBCopyCollectionName)} from projection with read model type {type}. Maybe it's missing the [{nameof(CopyProjectionToMongoDBAttribute)}] attribute on the class?", exception)
    {
    }
}
