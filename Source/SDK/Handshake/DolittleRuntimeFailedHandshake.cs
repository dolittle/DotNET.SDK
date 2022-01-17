// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Failures;

namespace Dolittle.SDK.Handshake;

/// <summary>
/// Exception that gets thrown when the Dolittle Runtime failed performing handshake.
/// </summary>
public class DolittleRuntimeFailedHandshake : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DolittleRuntimeFailedHandshake"/> class.
    /// </summary>
    /// <param name="failure">The <see cref="Failure"/>.</param>
    public DolittleRuntimeFailedHandshake(Failure failure)
        : base ($"Dolittle Runtime failed performing handshake. ({failure.Id}){failure.Reason}")
    {
    }
}
