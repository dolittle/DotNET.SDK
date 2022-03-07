// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Projections;

/// <summary>
/// Exception that gets thrown when trying to convert a <see cref="KeySelectorType"/> that does not have a known Contracts representation.
/// </summary>
public class UnknownKeySelectorType : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnknownKeySelectorType"/> class.
    /// </summary>
    /// <param name="keySelectorType">The <see cref="KeySelectorType"/> to be converted.</param>
    public UnknownKeySelectorType(KeySelectorType keySelectorType)
        : base($"The key selector type '{keySelectorType}' is unknown.")
    {
    }
}