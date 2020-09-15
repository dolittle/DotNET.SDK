// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.SDK.Artifacts.for_Artifacts.when_checking_if_has_for_artifact
{
    public class and_there_is_no_definition : given.no_associations
    {
        static given.artifact_type artifact;
        static bool result;

        Establish context = () => artifact = new given.artifact_type("cb2b8c0e-0ff5-4458-bbde-e305303faefb");

        Because of = () => result = artifacts.HasTypeFor(artifact);

        It should_not_have_an_association = () => result.ShouldBeFalse();
    }
}