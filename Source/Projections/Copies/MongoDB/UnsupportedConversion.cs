// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Projections.Copies.MongoDB;

/// <summary>
/// Exception that gets thrown when an unsupported Conversion is encountered.
/// </summary>
public class UnsupportedConversion : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnsupportedConversion"/> class.
    /// </summary>
    /// <param name="conversion"></param>
    public UnsupportedConversion(Conversion conversion)
        : base($"The conversion {conversion} is not supported")
    {
    }
}
