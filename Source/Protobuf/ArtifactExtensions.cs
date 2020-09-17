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
        /// <param name="source"><see cref="PbArtifact"/> to convert.</param>
        /// <param name="artifact">When the method returns, the converted <see cref="Artifact{TId}"/> if conversion was successful, otherwise default.</param>
        /// <param name="error">When the method returns, null if the conversion was successful, otherwise the error that caused the failure.</param>
        /// <typeparam name="TArtifact">The <see cref="Type" /> of the <see cref="IArtifact" />.</typeparam>
        /// <returns>A value indicating whether or not the conversion was successful.</returns>
        public static bool TryTo<TArtifact>(this PbArtifact source, out TArtifact artifact, out Exception error)
            where TArtifact : class, IArtifact
        {
            artifact = default;

            if (!source.Id.TryToGuid(out var id, out var idError))
            {
                error = new CouldNotConvertProtobufArtifact(typeof(TArtifact), source, idError.Message);
                return false;
            }

            var idType = typeof(TArtifact).GetProperty(nameof(IArtifact.Id)).PropertyType;
            var generationType = typeof(TArtifact).GetProperty(nameof(IArtifact.Generation)).PropertyType;
            try
            {
                object artifactId = Activator.CreateInstance(idType);
                var artifactIdValueProperty = idType.GetProperty(nameof(ArtifactId.Value));
                artifactIdValueProperty.SetValue(artifactId, id);

                Generation generation = artifact.Generation;
                var constructor = typeof(TArtifact).GetConstructor(new[] { idType, generationType });
                if (constructor == default)
                {
                    error = new ArtifactTypeDoesNotHaveConstructorWithIdAndGeneration(typeof(TArtifact), source);
                    return false;
                }

                artifact = constructor.Invoke(new object[] { artifactId, generation }) as TArtifact;
                if (artifact == default)
                {
                    error = new CouldNotConvertProtobufArtifact(typeof(TArtifact), source, "Could not create instance of artifact");
                    return false;
                }

                error = null;
                return true;
            }
            catch (MissingMethodException)
            {
                error = new ArtifactIdTypeDoesNotHaveDefaultParameterlessConstructor(typeof(TArtifact), source, idType);
                return false;
            }
            catch (Exception ex)
            {
                error = new CouldNotConvertProtobufArtifact(typeof(TArtifact), source, ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Convert a <see cref="PbArtifact"/> to a <typeparamref name="TArtifact"/>.
        /// </summary>
        /// <remarks>
        /// The method assumes that the <see cref="IArtifact" /> class has a constructor that takes in
        /// <see cref="ArtifactId" /> as first parameter and <see cref="Generation" /> as second parameter.
        /// It also assumes that the <see cref="ArtifactId" /> type in the <see cref="IArtifact" /> only has the default parameterless constructor.
        /// </remarks>
        /// <param name="source"><see cref="PbArtifact"/> to convert.</param>
        /// <typeparam name="TArtifact">The <see cref="Type" /> of the <see cref="IArtifact" />.</typeparam>
        /// <returns>The converted <see cref="Artifact{TId}"/>.</returns>
        public static TArtifact To<TArtifact>(this PbArtifact source)
            where TArtifact : class, IArtifact
            => source.TryTo<TArtifact>(out var artifact, out var error) ? artifact : throw error;

        /// <summary>
        /// Convert a <see cref="PbArtifact"/> to a tuple with an <see cref="ArtifactId" /> and a <see cref="Generation" />.
        /// </summary>
        /// <param name="source"><see cref="PbArtifact"/> to convert.</param>
        /// <param name="artifact">When the method returns, a tuple with <see cref="ArtifactId" /> and <see cref="Generation" /> if conversion was successful, otherwise null.</param>
        /// <param name="error">When the method returns, null if the conversion was successful, otherwise the error that caused the failure.</param>
        /// <returns>A value indicating whether or not the conversion was successful.</returns>
        public static bool TryToArtifact(this PbArtifact source, out (ArtifactId Id, Generation Generation) artifact, out Exception error)
        {
            artifact = default;
            if (!source.Id.TryTo<ArtifactId>(out var id, out error))
            {
                return false;
            }

            artifact = (id, source.Generation);
            return true;
        }

        /// <summary>
        /// Convert a <see cref="PbArtifact"/> to a tuple with an <see cref="ArtifactId" /> and a <see cref="Generation" />.
        /// </summary>
        /// <param name="source"><see cref="PbArtifact"/> to convert.</param>
        /// <returns>A tuple with <see cref="ArtifactId" /> and <see cref="Generation" />.</returns>
        public static (ArtifactId Id, Generation Generation) ToArtifact(this PbArtifact source)
            => source.TryToArtifact(out var artifact, out var error) ? artifact : throw error;
    }
}
