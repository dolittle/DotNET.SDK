// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.SDK.Artifacts.for_Artifacts.when_checking_if_has_for_artifact
{
    public class and_there_is_a_definition : given.one_association
    {
        static bool result;

        Because of = () => result = artifacts.HasTypeFor(associated_artifact);

        It should_have_an_association = () => result.ShouldBeTrue();
    }
}