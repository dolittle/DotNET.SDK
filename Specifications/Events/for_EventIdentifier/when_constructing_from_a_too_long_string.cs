// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Events.for_EventIdentifier
{
    public class when_constructing_from_a_too_long_string
    {
        static Exception exception;

        Because of = () => exception = Catch.Exception(() => new EventIdentifier("vl6rBCGG8Ga00kHEK+Nw4f6MKw8KJZW+D1SRtvXyJhKhazMfUPP0T5S65BdSKErX4Cwy1gHoDm0="));

        It should_throw_an_exception = () => exception.ShouldBeOfExactType<EventIdentifierStringIsInvalid>();
    }
}