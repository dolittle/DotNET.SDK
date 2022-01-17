// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.SDK.Artifacts.for_Artifacts.given
{
    public record artifact_type : Artifact<ArtifactId>
    {
        public artifact_type(ArtifactId id)
            : base(id)
        {
        }

        public artifact_type(ArtifactId id, Generation generation)
            : base(id, generation)
        {
        }
    }
}
