// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Artifacts;

namespace Dolittle.SDK.Protobuf.for_ArtifactExtensions.given
{
    public class artifact_type_without_id_and_generation_constructor : Artifact
    {
        public artifact_type_without_id_and_generation_constructor(ArtifactId id)
            : base(id, Generation.First)
        {
        }
    }
}
