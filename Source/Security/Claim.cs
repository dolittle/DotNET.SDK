// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.SDK.Security;

/// <summary>
/// Represents a Claim.
/// </summary>
/// <param name="Name">The Name of the claim.</param>
/// <param name="Value">The Value of the claim.</param>
/// <param name="ValueType">The type of the Value of the claim.</param>
public record Claim(string Name, string Value, string ValueType)
{

    /// <summary>
    /// Converts the <see cref="System.Security.Claims.Claim" /> instance into the corresponding <see cref="Claim" /> instance.
    /// </summary>
    /// <param name="claim"><see cref="System.Security.Claims.Claim"/> to convert.</param>
    /// <returns>a <see cref="Claim" /> instance.</returns>
    public static Claim FromDotnetClaim(System.Security.Claims.Claim claim)
        => claim == null
            ? null
            : new Claim(claim.Type, claim.Value, claim.ValueType);

    /// <summary>
    /// Converts the <see cref="Claim" /> instance into the corresponding <see cref="System.Security.Claims.Claim" /> instance.
    /// </summary>
    /// <returns>a <see cref="System.Security.Claims.Claim" /> instance.</returns>
    public System.Security.Claims.Claim ToDotnetClaim()
    {
        return new System.Security.Claims.Claim(Name, Value, ValueType);
    }
}
