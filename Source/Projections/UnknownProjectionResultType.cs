// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Projections;

/// <summary>
/// Exception that gets thrown when trying to convert a <see cref="ProjectionResultType"/> that does not have a known Contracts representation.
/// </summary>
public class UnknownProjectionResultType : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnknownProjectionResultType"/> class.
    /// </summary>
    /// <param name="resultType">The <see cref="ProjectionResultType"/> to be converted.</param>
    public UnknownProjectionResultType(ProjectionResultType resultType)
        : base($"The projection result type '{resultType}' is unknown.")
    {
    }
}
