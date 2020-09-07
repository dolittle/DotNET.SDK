// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.SDK.Artifacts.for_ArtifactTypeMap
{
    public class when_associating : given.no_associations
    {
        static Artifact associated_artifact;
        static Type associated_type;

        Establish context = () =>
        {
            associated_artifact = new Artifact(Guid.NewGuid());
            associated_type = typeof(string);
        };

        Because of = () => artifacts.Associate(associated_type, associated_artifact);

        It should_hold_a_associated_artifact_by_generic_type = () => artifacts.HasFor<string>().ShouldBeTrue();
        It should_hold_the_correct_associated_artifact_by_generic_type = () => artifacts.GetFor<string>().ShouldEqual(associated_artifact);
        It should_hold_the_associated_artifact_by_type = () => artifacts.HasFor(typeof(string)).ShouldBeTrue();
        It should_hold_the_correct_associated_artifact_by_type = () => artifacts.GetFor(typeof(string)).ShouldEqual(associated_artifact);
        It should_hold_the_associated_type_by_artifact = () => artifacts.HasTypeFor(associated_artifact).ShouldBeTrue();
        It should_hold_the_correct_associated_type_by_artifact = () => artifacts.GetTypeFor(associated_artifact).ShouldEqual(associated_type);
        It should_hold_the_associated_type_by_artifact_id = () => artifacts.HasTypeFor(associated_artifact.Id).ShouldBeTrue();
        It should_hold_the_correct_associated_type_by_artifact_id = () => artifacts.GetTypeFor(associated_artifact.Id).ShouldEqual(associated_type);
    }
}