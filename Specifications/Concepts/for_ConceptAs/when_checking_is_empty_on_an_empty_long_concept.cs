// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.SDK.Concepts.for_ConceptAs
{
    [Subject(typeof(ConceptAs<>))]
    public class when_checking_is_empty_on_an_empty_long_concept : Dolittle.SDK.Concepts.given.concepts
    {
        static bool is_empty;

        Establish context = () => is_empty = empty_long_value.IsEmpty();

        It should_be_empty = () => is_empty.ShouldBeTrue();
    }
}