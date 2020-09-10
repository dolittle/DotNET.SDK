// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Artifacts;
using Machine.Specifications;
using PbArtifact = Dolittle.Artifacts.Contracts.Artifact;

namespace Dolittle.SDK.Protobuf.for_ArtifactExtensions.when_converting_to_generic_artifact
{
    public class a_protobuf_artifact_type_with_base_artifact_id
    {
        static Guid id;
        static Generation generation;
        static PbArtifact artifact;
        static given.artifact_type_with_base_artifact_id result;

        Establish context = () =>
        {
            id = Guid.Parse("226429ad-06db-4868-b61a-bff9a7057faa");
            generation = 3;
            artifact = new PbArtifact { Id = id.ToProtobuf(), Generation = generation };
        };

        Because of = () => result = artifact.To<given.artifact_type_with_base_artifact_id>();

        It should_have_the_same_id = () => result.Id.Value.ShouldEqual(id);
        It should_have_the_same_id_type = () => result.Id.GetType().ShouldEqual(typeof(ArtifactId));
        It should_have_the_same_generation = () => result.Generation.ShouldEqual(generation);
    }
}