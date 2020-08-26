// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Events.for_EventIdentifier
{
    public class when_constructing_from_components
    {
        static EventIdentifier identifier;

        Because of = () => identifier = new EventIdentifier(Guid.Parse("e9338cc3-a59f-440e-81a9-c4597955277d"), Guid.Parse("6b705ed8-e189-4741-b932-103f0c11d0dc"), 985862872);

        It should_have_the_correct_value = () => identifier.Value.ShouldEqual("w4wz6Z+lDkSBqcRZeVUnfdhecGuJ4UFHuTIQPwwR0NzYEsM6AAAAAA==");

        It should_decompose_into_the_correct_components = () =>
        {
            identifier.Decompose(out var microservice, out var tenant, out var sequenceNumber);
            microservice.Value.ShouldEqual(Guid.Parse("e9338cc3-a59f-440e-81a9-c4597955277d"));
            tenant.Value.ShouldEqual(Guid.Parse("6b705ed8-e189-4741-b932-103f0c11d0dc"));
            sequenceNumber.Value.ShouldEqual<ulong>(985862872);
        };
    }
}