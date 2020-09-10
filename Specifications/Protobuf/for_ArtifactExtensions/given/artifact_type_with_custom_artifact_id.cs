// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Artifacts;

namespace Dolittle.SDK.Protobuf.for_ArtifactExtensions.given
{
    public class artifact_type_with_custom_artifact_id : Artifact
    {
        public artifact_type_with_custom_artifact_id(custom_artifact_id id, Generation generation)
            : base(id, generation)
        {
        }
    }
}
