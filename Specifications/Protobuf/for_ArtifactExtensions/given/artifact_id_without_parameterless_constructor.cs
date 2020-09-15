// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Artifacts;

namespace Dolittle.SDK.Protobuf.for_ArtifactExtensions.given
{
    public class artifact_id_without_parameterless_constructor : ArtifactId
    {
        public artifact_id_without_parameterless_constructor(ArtifactId id)
            : base()
        {
            Value = id.Value;
        }
    }
}