// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Artifacts;
using PbArtifact = Dolittle.Artifacts.Contracts.Artifact;

namespace Dolittle.SDK.Protobuf
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
        /// Convert a <see cref="PbArtifact"/> to a <typeparamref name="TArtifact"/> .
        /// </summary>
        /// <param name="artifact"><see cref="PbArtifact"/> to convert.</param>
        /// <typeparam name="TArtifact">The <see cref="Type" /> of the <see cref="Artifact" />.</typeparam>
        /// <typeparam name="TArtifactId">The <see cref="Type" /> of the unique identifier of the <see cref="Artifact" />.</typeparam>
        /// <returns>The converted <see cref="Artifact"/>.</returns>
        public static TArtifact To<TArtifact, TArtifactId>(this PbArtifact artifact)
            where TArtifact : Artifact
            where TArtifactId : ArtifactId, new()
        {
            var constructor = typeof(TArtifact).GetConstructor(new[] { typeof(TArtifactId), typeof(Generation) });
            var instance = constructor.Invoke(new object[] { artifact.Id.To<TArtifactId>(), new Generation { Value = artifact.Generation } });
            return instance as TArtifact;
        }

        /// <summary>
        /// Convert a <see cref="PbArtifact"/> to a tuple with an <see cref="ArtifactId" /> and a <see cref="Generation" />.
        /// </summary>
        /// <param name="artifact"><see cref="PbArtifact"/> to convert.</param>
        /// <returns>The a tuple with <see cref="ArtifactId" /> and <see cref="Generation" />.</returns>
        public static (ArtifactId id, Generation generation) ToArtifact(this PbArtifact artifact)
            => (artifact.Id.To<ArtifactId>(), artifact.Generation);
    }
}