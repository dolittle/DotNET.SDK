// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.SDK.Artifacts.for_Artifacts.when_getting_type
{
    public class and_there_is_a_definition : given.one_association
    {
        static Type result;

        Because of = () => result = artifacts.GetTypeFor(associated_artifact);

        It should_return_the_type = () => result.ShouldEqual(associated_type);
    }
}