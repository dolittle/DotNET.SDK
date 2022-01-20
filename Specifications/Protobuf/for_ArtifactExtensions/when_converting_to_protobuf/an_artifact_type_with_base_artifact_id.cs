// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Artifacts.Contracts;
using Machine.Specifications;

namespace Dolittle.SDK.Protobuf.for_ArtifactExtensions.when_converting_to_protobuf;

public class an_artifact_type_with_base_artifact_id
{
    static given.artifact_type_with_base_artifact_id artifact;
    static Artifact result;

    Establish context = () => artifact = new given.artifact_type_with_base_artifact_id("24faf6b5-3d64-4314-b1e4-97f0ff8bf942", 1);

    Because of = () => result = artifact.ToProtobuf();

    It should_create_the_protobuf_artifact = () => result.ShouldNotBeNull();
    It should_have_the_same_id = () => result.Id.ToGuid().ShouldEqual(artifact.Id.Value);
    It should_have_the_same_generation = () => result.Generation.ShouldEqual(artifact.Generation.Value);
}