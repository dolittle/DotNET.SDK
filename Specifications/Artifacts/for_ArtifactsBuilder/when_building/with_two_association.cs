// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.SDK.Artifacts.for_ArtifactsBuilder.when_building
{
    public class with_two_association : given.a_builder
    {
        static Type first_type;
        static Type second_type;
        static Artifact first_artifact;
        static Artifact second_artifact;
        static IArtifacts result;
        static Artifact first_association;
        static Artifact second_association;

        Establish context = () =>
        {
            first_type = typeof(string);
            second_type = typeof(StringComparer);
            first_artifact = new Artifact("4ea61dd7-ad89-444c-9c6e-1c1242899568", 4);
            second_artifact = new Artifact("99bae4dd-1329-4a06-8c84-b7a7497e3b8f", 2);
        };

        Because of = () =>
        {
            builder.Associate(first_type, first_artifact);
            builder.Associate(second_type, second_artifact);
            result = builder.Build();
            first_association = result.GetFor(first_type);
            second_association = result.GetFor(second_type);
        };

        It should_return_an_instance = () => result.ShouldNotBeNull();
        It should_associate_identifier_for_first_type = () => first_association.Id.ShouldEqual(first_artifact.Id);
        It should_associate_generation_for_first_type = () => first_association.Generation.ShouldEqual(first_artifact.Generation);
        It should_associate_identifier_for_second_type = () => second_association.Id.ShouldEqual(second_artifact.Id);
        It should_associate_generation_for_second_type = () => second_association.Generation.ShouldEqual(second_artifact.Generation);
    }
}