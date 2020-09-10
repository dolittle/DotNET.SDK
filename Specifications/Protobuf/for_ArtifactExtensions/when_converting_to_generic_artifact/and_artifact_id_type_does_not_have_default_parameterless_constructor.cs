// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Artifacts;
using Machine.Specifications;
using PbArtifact = Dolittle.Artifacts.Contracts.Artifact;

namespace Dolittle.SDK.Protobuf.for_ArtifactExtensions.when_converting_to_generic_artifact
{
    public class and_artifact_id_type_does_not_have_default_parameterless_constructor
    {
        static Guid id;
        static Generation generation;
        static PbArtifact artifact;
        static Exception result;

        Establish context = () =>
        {
            id = Guid.Parse("226429ad-06db-4868-b61a-bff9a7057faa");
            generation = 3;
            artifact = new PbArtifact { Id = id.ToProtobuf(), Generation = generation };
        };

        Because of = () => result = Catch.Exception(() => artifact.To<given.artifact_type_with_custom_artifact_id_without_parameterless_constructor>());

        It should_fail_because_missing_parameterless_constructor = () => result.ShouldBeOfExactType<ArtifactIdDoesNotHaveDefaultParameterlessConstructor>();
    }
}