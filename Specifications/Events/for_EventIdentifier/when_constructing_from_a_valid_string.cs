// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Events.for_EventIdentifier
{
    public class when_constructing_from_a_valid_string
    {
        static EventIdentifier identifier;

        Because of = () => identifier = new EventIdentifier("qTXfie442kGNN1SgDWrg5k+QarP7zplGp6ON7MrtiiJLJYQqAAAAAA==");

        It should_have_the_correct_value = () => identifier.Value.ShouldEqual("qTXfie442kGNN1SgDWrg5k+QarP7zplGp6ON7MrtiiJLJYQqAAAAAA==");

        It should_decompose_into_the_correct_components = () =>
        {
            identifier.Decompose(out var microservice, out var tenant, out var sequenceNumber);
            microservice.Value.ShouldEqual(Guid.Parse("89df35a9-38ee-41da-8d37-54a00d6ae0e6"));
            tenant.Value.ShouldEqual(Guid.Parse("b36a904f-cefb-4699-a7a3-8deccaed8a22"));
            sequenceNumber.Value.ShouldEqual<ulong>(713303371);
        };
    }
}