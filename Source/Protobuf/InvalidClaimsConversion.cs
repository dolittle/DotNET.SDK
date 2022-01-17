// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Security;
using Claim = Dolittle.Security.Contracts.Claim;

namespace Dolittle.SDK.Protobuf;

/// <summary>
/// Exception that gets thrown when an error occurs converting between <see cref="Claims"/> and <see cref="Claim"/>.
/// </summary>
public class InvalidClaimsConversion : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidClaimsConversion"/> class.
    /// </summary>
    /// <param name="details">The details on why conversion failed.</param>
    public InvalidClaimsConversion(string details)
        : base($"Could not convert claims because {details}")
    {
    }
}