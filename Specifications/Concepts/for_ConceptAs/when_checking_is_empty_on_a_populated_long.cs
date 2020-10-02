// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.SDK.Concepts.for_ConceptAs
{
    [Subject(typeof(ConceptAs<>))]
    public class when_checking_is_empty_on_a_populated_long : Dolittle.SDK.Concepts.given.concepts
    {
        static bool is_empty;

        Establish context = () => is_empty = value_as_a_long.IsEmpty();

        It should_not_be_empty = () => is_empty.ShouldBeFalse();
    }
}