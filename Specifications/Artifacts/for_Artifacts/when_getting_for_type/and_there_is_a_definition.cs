// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.SDK.Artifacts.for_Artifacts.when_getting_for_type
{
    public class and_there_is_a_definition : given.one_association
    {
        static given.artifact_type result;

        Because of = () => result = artifacts.GetFor(associated_type);

        It should_return_the_artifact = () => result.ShouldEqual(associated_artifact);
    }
}