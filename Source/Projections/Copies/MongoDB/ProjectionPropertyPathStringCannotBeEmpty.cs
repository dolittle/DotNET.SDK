// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Projections.Copies.MongoDB;

/// <summary>
/// Exception that gets thrown if a <see cref="PropertyPath"/> is null or empty.
/// </summary>
public class ProjectionPropertyPathStringCannotBeEmpty : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionPropertyPathStringCannotBeEmpty"/>.
    /// </summary>
    public ProjectionPropertyPathStringCannotBeEmpty()
        : base($"A projection property path string cannot be null or empty")
    {
    }
}
