// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.SDK.Events.for_EventType.when_checking_equality;

public class and_they_are_equal
{
    static EventType first;
    static EventType second;

    Establish context = () =>
    {
        first = new EventType("0f025f6c-41be-4192-895d-c1c771832bae", 2, "alias");
        second = new EventType("0f025f6c-41be-4192-895d-c1c771832bae", 2, "alias");
    };
    
    static bool result;

    Because of = () => result = first.Equals(second);

    It should_return_true = () => result.ShouldBeTrue();
}