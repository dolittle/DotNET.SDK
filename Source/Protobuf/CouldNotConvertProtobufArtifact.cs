// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Artifacts;
using PbArtifact = Dolittle.Artifacts.Contracts.Artifact;

namespace Dolittle.SDK.Protobuf
{
    /// <summary>
    /// Exception that gets thrown when a protobuf artifact could not be converted to an <see cref="Artifact" /> of a specific type.
    /// </summary>
    public class CouldNotConvertProtobufArtifact : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CouldNotConvertProtobufArtifact"/> class.
        /// </summary>
        /// <param name="expectedArtifactType">The <see cref="Type" /> of <see cref="Artifact" /> the protobuf artifact should be converted to.</param>
        /// <param name="artifact">The <see cref="PbArtifact" />.</param>
        public CouldNotConvertProtobufArtifact(Type expectedArtifactType, PbArtifact artifact)
            : base($"Could not convert artifact ({artifact.Id?.ToGuid()}, {artifact.Generation}) to artifact of type {expectedArtifactType}")
        {
        }
    }
}