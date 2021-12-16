// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Artifacts.for_Artifacts.given
{
    public class artifact_types : StrictArtifacts<artifact_type, ArtifactId>
    {
        public static Exception CannotAssociateMultipleArtifactsWithTypeException = new Exception("CannotAssociateMultipleArtifactsWithType");
        public static Exception CannotAssociateMultipleTypesWithArtifactException = new Exception("CannotAssociateMultipleTypesWithArtifact");
        public static Exception NoArtifactAssociatedWithTypeException = new Exception("eNoArtifactAssociatedWithType");
        public static Exception NoTypeAssociatedWithArtifactException = new Exception("NoTypeAssociatedWithArtifact");

        public artifact_types()
            : base(Moq.Mock.Of<ILogger<StrictArtifacts<artifact_type, ArtifactId>>>())
        {
        }

        protected override Exception CreateCannotAssociateMultipleArtifactsWithType(Type type, artifact_type artifact, artifact_type existing)
            => CannotAssociateMultipleArtifactsWithTypeException;

        protected override Exception CreateCannotAssociateMultipleTypesWithArtifact(artifact_type artifact, Type type, Type existing)
            => CannotAssociateMultipleTypesWithArtifactException;

        protected override Exception CreateNoArtifactAssociatedWithType(Type type)
            => NoArtifactAssociatedWithTypeException;

        protected override Exception CreateNoTypeAssociatedWithArtifact(artifact_type artifact)
            => NoTypeAssociatedWithArtifactException;
    }
}
