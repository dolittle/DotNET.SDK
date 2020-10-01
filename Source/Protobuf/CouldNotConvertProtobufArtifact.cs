// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Artifacts;
using PbArtifact = Dolittle.Artifacts.Contracts.Artifact;

namespace Dolittle.SDK.Protobuf
{
    /// <summary>
    /// Exception that gets thrown when a protobuf artifact could not be converted to an <see cref="Artifact{TId}" /> of a specific type.
    /// </summary>
    public class CouldNotConvertProtobufArtifact : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CouldNotConvertProtobufArtifact"/> class.
        /// </summary>
        /// <param name="expectedArtifactType">The <see cref="Type" /> of <see cref="Artifact{TId}" /> the protobuf artifact should be converted to.</param>
        /// <param name="artifact">The <see cref="PbArtifact" />.</param>
        /// <param name="details">The details of the exception.</param>
        public CouldNotConvertProtobufArtifact(Type expectedArtifactType, PbArtifact artifact, string details)
            : base($"Could not convert artifact ({artifact.Id?.ToGuid()}, {artifact.Generation}) to an artifact of type {expectedArtifactType}. {details}")
        {
        }
    }
}
