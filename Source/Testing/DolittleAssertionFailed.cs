// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Testing;

/// <summary>
/// Exception that gets thrown when there is a Dolittle related assertion that failed.
/// </summary>
public class DolittleAssertionFailed : Exception
{
    /// <summary>
    /// Throws a <see cref="DolittleAssertionFailed"/> exception.
    /// </summary>
    /// <param name="reason">The assertion message.</param>
    public static void Throw(string reason) => throw new DolittleAssertionFailed(reason);

    /// <summary>
    /// Initializes a new instance of the <see cref="DolittleAssertionFailed"/> class.
    /// </summary>
    /// <param name="reason">The assertion message.</param>
    public DolittleAssertionFailed(string reason)
        : base(reason)
    {
    }
}
