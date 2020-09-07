// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.SDK.Artifacts.for_Artifacts.when_getting_type
{
    public class and_there_is_no_definition : given.no_associations
    {
        static Exception exception;

        Because of = () => exception = Catch.Exception(() => artifacts.GetTypeFor("276244a5-ee98-4135-ba3b-a282b41094a1"));

        It should_fail_because_of_unknown_type = () => exception.ShouldBeOfExactType<UnknownType>();
    }
}