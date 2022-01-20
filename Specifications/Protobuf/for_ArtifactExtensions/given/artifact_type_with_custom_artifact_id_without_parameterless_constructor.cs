// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Artifacts;

namespace Dolittle.SDK.Protobuf.for_ArtifactExtensions.given;

public record artifact_type_with_custom_artifact_id_without_parameterless_constructor : Artifact<artifact_id_without_parameterless_constructor>
{
    public artifact_type_with_custom_artifact_id_without_parameterless_constructor(artifact_id_without_parameterless_constructor id, Generation generation)
        : base(id, generation)
    {
    }
}