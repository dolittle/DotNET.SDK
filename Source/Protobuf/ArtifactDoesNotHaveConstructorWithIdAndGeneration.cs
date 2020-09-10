// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Artifacts;
using PbArtifact = Dolittle.Artifacts.Contracts.Artifact;

namespace Dolittle.SDK.Protobuf
{
    /// <summary>
    /// Exception that gets thrown when an <see cref="IArtifact" /> does not have a constructor where the first parameter is a <see cref="ArtifactId" /> and the second is a <see cref="Generation" />..
    /// </summary>
    public class ArtifactDoesNotHaveConstructorWithIdAndGeneration : CouldNotConvertProtobufArtifact
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArtifactDoesNotHaveConstructorWithIdAndGeneration"/> class.
        /// </summary>
        /// <param name="expectedArtifactType">The <see cref="Type" /> of <see cref="IArtifact" /> the protobuf artifact should be converted to.</param>
        /// <param name="artifact">The <see cref="PbArtifact" />.</param>
        public ArtifactDoesNotHaveConstructorWithIdAndGeneration(Type expectedArtifactType, PbArtifact artifact)
            : base(expectedArtifactType, artifact, $"Artifact type {expectedArtifactType} does not have a constructor where the first parameter is a {typeof(ArtifactId)} and second parameter is a {typeof(Generation)}")
        {
        }
    }
}
