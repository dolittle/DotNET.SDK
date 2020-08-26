// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Artifacts.for_ArtifactTypeMap.given
{
    public class an_empty_artifact_type_map
    {
        protected static ArtifactTypeMap artifact_type_map;

        Establish context = () => artifact_type_map = new ArtifactTypeMap();
    }
}