// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Artifacts;
using Dolittle.SDK.Protobuf.for_ArtifactExtensions.given;
using Machine.Specifications;
using PbArtifact = Dolittle.Artifacts.Contracts.Artifact;

namespace Dolittle.SDK.Protobuf.for_ArtifactExtensions.when_converting_to_artifact;

public class a_protobuf_artifact
{
    static Guid id;
    static Generation generation;
    static PbArtifact artifact;
    static (custom_artifact_id Id, Generation Generation) result;

    Establish context = () =>
    {
        id = Guid.Parse("8db37cb7-aada-4792-99ec-8bb9f8465ec1");
        generation = 1;
        artifact = new PbArtifact { Id = id.ToProtobuf(), Generation = generation };
    };

    Because of = () => result = artifact.ToArtifact<custom_artifact_id>();

    It should_have_the_same_id = () => result.Id.Value.ShouldEqual(id);
    It should_have_the_same_generation = () => result.Generation.ShouldEqual(generation);
}