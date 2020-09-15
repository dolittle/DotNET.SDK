// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Artifacts;
using PbArtifact = Dolittle.Artifacts.Contracts.Artifact;

namespace Dolittle.SDK.Protobuf
{
    /// <summary>
    /// Conversion extensions for converting between <see cref="IArtifact"/> and <see cref="PbArtifact"/>.
    /// </summary>
    public static class ArtifactExtensions
    {
        /// <summary>
        /// Convert a <see cref="IArtifact"/> to a <see cref="PbArtifact"/>.
        /// </summary>
        /// <param name="artifact"><see cref="IArtifact"/> to convert.</param>
        /// <returns>The converted <see cref="PbArtifact"/>.</returns>
        public static PbArtifact ToProtobuf(this IArtifact artifact)
            => new PbArtifact { Id = artifact.Id.ToProtobuf(), Generation = artifact.Generation.Value };

        /// <summary>
        /// Convert a <see cref="PbArtifact"/> to a <typeparamref name="TArtifact"/>.
        /// </summary>
        /// <remarks>
        /// The method assumes that the <see cref="IArtifact" /> class has a constructor that takes in
        /// <see cref="ArtifactId" /> as first parameter and <see cref="Generation" /> as second parameter.
        /// It also assumes that the <see cref="ArtifactId" /> type in the <see cref="IArtifact" /> only has the default parameterless constructor.
        /// </remarks>
        /// <param name="artifact"><see cref="PbArtifact"/> to convert.</param>
        /// <typeparam name="TArtifact">The <see cref="Type" /> of the <see cref="IArtifact" />.</typeparam>
        /// <returns>The converted <see cref="Artifact{TId}"/>.</returns>
        public static TArtifact To<TArtifact>(this PbArtifact artifact)
            where TArtifact : class, IArtifact
        {
            var idType = typeof(TArtifact).GetProperty(nameof(IArtifact.Id)).PropertyType;
            var generationType = typeof(TArtifact).GetProperty(nameof(IArtifact.Generation)).PropertyType;
            object artifactId;
            try
            {
                artifactId = Activator.CreateInstance(idType);
                var artifactIdValueProperty = idType.GetProperty(nameof(ArtifactId.Value));
                artifactIdValueProperty.SetValue(artifactId, artifact.Id.ToGuid());
            }
            catch (MissingMethodException)
            {
                throw new ArtifactIdTypeDoesNotHaveDefaultParameterlessConstructor(typeof(TArtifact), artifact, idType);
            }

            Generation generation = artifact.Generation;
            var constructor = typeof(TArtifact).GetConstructor(new[] { idType, generationType });
            if (constructor == default) throw new ArtifactTypeDoesNotHaveConstructorWithIdAndGeneration(typeof(TArtifact), artifact);

            var instance = constructor.Invoke(new object[] { artifactId, generation });
            if (instance == default) throw new CouldNotConvertProtobufArtifact(typeof(TArtifact), artifact, "Could not create instance of artifact");
            return instance as TArtifact;
        }

        /// <summary>
        /// Convert a <see cref="PbArtifact"/> to a tuple with an <see cref="ArtifactId" /> and a <see cref="Generation" />.
        /// </summary>
        /// <param name="artifact"><see cref="PbArtifact"/> to convert.</param>
        /// <returns>The a tuple with <see cref="ArtifactId" /> and <see cref="Generation" />.</returns>
        public static (ArtifactId Id, Generation Generation) ToArtifact(this PbArtifact artifact)
            => (artifact.Id.To<ArtifactId>(), artifact.Generation);
    }
}
