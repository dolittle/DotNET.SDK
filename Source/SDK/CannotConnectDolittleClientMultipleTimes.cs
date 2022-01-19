// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Builders;

namespace Dolittle.SDK;

/// <summary>
/// Exception that gets thrown when calling <see cref="IDolittleClient.Connect(ConfigureDolittleClient,System.Threading.CancellationToken)"/> more than once.
/// </summary>
public class CannotConnectDolittleClientMultipleTimes : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CannotConnectDolittleClientMultipleTimes"/> class.
    /// </summary>
    public CannotConnectDolittleClientMultipleTimes()
        : base($"Cannot call {nameof(DolittleClient.Connect)}() more than once on the Dolittle Client.")
    {
    }
}
