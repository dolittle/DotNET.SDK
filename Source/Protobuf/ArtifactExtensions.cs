// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.SDK.Artifacts;
using PbArtifact = Dolittle.Artifacts.Contracts.Artifact;

namespace Dolittle.SDK.Protobuf.Events
{
    /// <summary>
    /// Conversion extensions for converting between <see cref="Artifact"/> and <see cref="PbArtifact"/>.
    /// </summary>
    public static class ArtifactExtensions
    {
        /// <summary>
        /// Convert a <see cref="Artifact"/> to a <see cref="PbArtifact"/>.
        /// </summary>
        /// <param name="artifact"><see cref="Artifact"/> to convert.</param>
        /// <returns>The converted <see cref="PbArtifact"/>.</returns>
        public static PbArtifact ToProtobuf(this Artifact artifact)
            => new PbArtifact { Id = artifact.Id.ToProtobuf(), Generation = artifact.Generation.Value };

        /// <summary>
        /// Convert a <see cref="PbArtifact"/> to a <see cref="Artifact"/>.
        /// </summary>
        /// <param name="artifact"><see cref="PbArtifact"/> to convert.</param>
        /// <typeparam name="TArtifact">The <see cref="Type" /> of the <see cref="Artifact" />.</typeparam>
        /// <typeparam name="TArtifactId">The <see cref="Type" /> of the unique identifier of the <see cref="Artifact" />.</typeparam>
        /// <returns>The converted <see cref="Artifact"/>.</returns>
        public static TArtifact To<TArtifact, TArtifactId>(this PbArtifact artifact)
            where TArtifact : Artifact, new()
            where TArtifactId : ArtifactId, new()
            => new TArtifact { Id = artifact.Id.To<TArtifactId>(),  }

        /// <summary>
        /// Convert from <see cref="Claims"/> to an <see cref="IEnumerable{T}"/> of <see cref="PbArtifact"/>.
        /// </summary>
        /// <param name="claims"><see cref="Claims"/> to convert from.</param>
        /// <returns>The converted <see cref="IEnumerable{T}"/> of <see cref="PbArtifact"/>.</returns>
        public static IEnumerable<PbArtifact> ToProtobuf(this Claims claims) => claims.Select(ToProtobuf);

        /// <summary>
        /// Convert from an <see cref="IEnumerable{T}"/> of <see cref="PbArtifact"/> to <see cref="Claims"/>.
        /// </summary>
        /// <param name="claims"><see cref="IEnumerable{T}"/> of <see cref="PbArtifact"/> to convert from.</param>
        /// <returns>The converted <see cref="Claims"/>.</returns>
        public static Claims ToClaims(this IEnumerable<PbArtifact> claims) => new Claims(claims.Select(ToClaim));
    }
}