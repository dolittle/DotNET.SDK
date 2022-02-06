// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Projections.Copies;

/// <summary>
/// Exception that gets thrown if a <see cref="PropertyPath"/> is null or empty.
/// </summary>
public class PropertyPathStringCannotBeEmpty : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyPathStringCannotBeEmpty"/>.
    /// </summary>
    public PropertyPathStringCannotBeEmpty()
        : base($"A projection property path string cannot be null or empty")
    {
    }
}
