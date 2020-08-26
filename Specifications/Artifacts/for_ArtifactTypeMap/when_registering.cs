// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Artifacts.for_ArtifactTypeMap
{
    public class when_registering : given.an_empty_artifact_type_map
    {
        static Artifact registered_artifact;
        static Type registered_type;

        Establish context = () =>
        {
            registered_artifact = Artifact.New();
            registered_type = typeof(string);
        };

        Because of = () => artifact_type_map.Register(registered_artifact, registered_type);

        It should_hold_the_registered_artifact_by_generic_type = () => artifact_type_map.GetArtifactFor<string>().ShouldEqual(registered_artifact);
        It should_hold_the_registered_artifact_by_type = () => artifact_type_map.GetArtifactFor(typeof(string)).ShouldEqual(registered_artifact);
        It should_hold_the_registered_type_by_artifact = () => artifact_type_map.GetTypeFor(registered_artifact).ShouldEqual(registered_type);
    }
}