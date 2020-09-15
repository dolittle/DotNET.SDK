// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Artifacts;
using PbArtifact = Dolittle.Artifacts.Contracts.Artifact;

namespace Dolittle.SDK.Protobuf
{
    /// <summary>
    /// Exception that gets thrown when the <see cref="Type" /> of an <see cref="ArtifactId" /> does not have a default parameterless constructor.
    /// </summary>
    public class ArtifactIdTypeDoesNotHaveDefaultParameterlessConstructor : CouldNotConvertProtobufArtifact
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArtifactIdTypeDoesNotHaveDefaultParameterlessConstructor"/> class.
        /// </summary>
        /// <param name="expectedArtifactType">The <see cref="Type" /> of <see cref="IArtifact" /> the protobuf artifact should be converted to.</param>
        /// <param name="artifact">The <see cref="PbArtifact" />.</param>
        /// <param name="artifactIdType">The <see cref="Type" /> of the <see cref="ArtifactId" />.</param>
        public ArtifactIdTypeDoesNotHaveDefaultParameterlessConstructor(Type expectedArtifactType, PbArtifact artifact, Type artifactIdType)
            : base(expectedArtifactType, artifact, $"{artifactIdType} does not have a default parameterless constructor")
        {
        }
    }
}
