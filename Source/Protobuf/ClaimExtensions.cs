// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Dolittle.SDK.Security;
using PbClaim = Dolittle.Security.Contracts.Claim;

namespace Dolittle.SDK.Protobuf
{
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
            => new PbClaim { Key = claim.Name, Value = claim.Value, ValueType = claim.ValueType };

        /// <summary>
        /// Convert a <see cref="PbClaim"/> to a <see cref="Claim"/>.
        /// </summary>
        /// <param name="claim"><see cref="PbClaim"/> to convert.</param>
        /// <returns>The converted <see cref="Claim"/>.</returns>
        public static Claim ToClaim(this PbClaim claim)
            => new Claim(claim.Key, claim.Value, claim.ValueType);

        /// <summary>
        /// Convert from <see cref="Claims"/> to an <see cref="IEnumerable{T}"/> of <see cref="PbClaim"/>.
        /// </summary>
        /// <param name="claims"><see cref="Claims"/> to convert from.</param>
        /// <returns>The converted <see cref="IEnumerable{T}"/> of <see cref="PbClaim"/>.</returns>
        public static IEnumerable<PbClaim> ToProtobuf(this Claims claims) => claims.Select(ToProtobuf);

        /// <summary>
        /// Convert from an <see cref="IEnumerable{T}"/> of <see cref="PbClaim"/> to <see cref="Claims"/>.
        /// </summary>
        /// <param name="claims"><see cref="IEnumerable{T}"/> of <see cref="PbClaim"/> to convert from.</param>
        /// <returns>The converted <see cref="Claims"/>.</returns>
        public static Claims ToClaims(this IEnumerable<PbClaim> claims) => new Claims(claims.Select(ToClaim));
    }
}