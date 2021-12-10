// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.SDK.Security;
using PbClaim = Dolittle.Security.Contracts.Claim;

namespace Dolittle.SDK.Protobuf;

/// <summary>
/// Conversion extensions for converting between <see cref="Claim"/> and <see cref="PbClaim"/>.
/// </summary>
public static class ClaimExtensions
{
    /// <summary>
    /// Convert a <see cref="Claim"/> to a <see cref="PbClaim"/>.
    /// </summary>
    /// <param name="claim"><see cref="Claim"/> to convert.</param>
    /// <returns>The converted <see cref="PbClaim"/>.</returns>
    public static PbClaim ToProtobuf(this Claim claim)
        => new() { Key = claim.Name, Value = claim.Value, ValueType = claim.ValueType };

    /// <summary>
    /// Convert a <see cref="PbClaim"/> to a <see cref="Claim"/>.
    /// </summary>
    /// <param name="claim"><see cref="PbClaim"/> to convert.</param>
    /// <returns>The converted <see cref="Claim"/>.</returns>
    public static Claim ToClaim(this PbClaim claim)
        => new(claim.Key, claim.Value, claim.ValueType);

    /// <summary>
    /// Convert from <see cref="Claims"/> to an <see cref="IEnumerable{T}"/> of <see cref="PbClaim"/>.
    /// </summary>
    /// <param name="claims"><see cref="Claims"/> to convert from.</param>
    /// <returns>The converted <see cref="IEnumerable{T}"/> of <see cref="PbClaim"/>.</returns>
    public static IEnumerable<PbClaim> ToProtobuf(this Claims claims) => claims.Select(ToProtobuf);

    /// <summary>
    /// Convert from an <see cref="IEnumerable{T}"/> of <see cref="PbClaim"/> to <see cref="Claims"/>.
    /// </summary>
    /// <param name="source"><see cref="IEnumerable{T}"/> of <see cref="PbClaim"/> to convert from.</param>
    /// <param name="claims">When the method returns, the converted <see cref="Claims"/> if conversion was successful, otherwise null.</param>
    /// <param name="error">When the method returns, null if the conversion was successful, otherwise the error that caused the failure.</param>
    /// <returns>A value indicating whether or not the conversion was successful.</returns>
    public static bool TryToClaims(this IEnumerable<PbClaim> source, out Claims claims, out Exception error)
    {
        claims = null;
        if (source == null)
        {
            error = new InvalidClaimsConversion("list was null");
            return false;
        }

        var list = new List<Claim>();
        foreach (var claim in source)
        {
            if (claim == null)
            {
                error = new InvalidClaimsConversion("one of the claims was null");
                return false;
            }

            if (string.IsNullOrEmpty(claim.Key))
            {
                error = new InvalidClaimsConversion("one of the claims was missing the key");
                return false;
            }

            list.Add(new Claim(claim.Key, claim.Value, claim.ValueType));
        }

        claims = new Claims(list);
        error = null;
        return true;
    }

    /// <summary>
    /// Convert from an <see cref="IEnumerable{T}"/> of <see cref="PbClaim"/> to <see cref="Claims"/>.
    /// </summary>
    /// <param name="source"><see cref="IEnumerable{T}"/> of <see cref="PbClaim"/> to convert from.</param>
    /// <returns>The converted <see cref="Claims"/>.</returns>
    public static Claims ToClaims(this IEnumerable<PbClaim> source)
        => source.TryToClaims(out var claims, out var error)
            ? claims
            : throw error;
}
