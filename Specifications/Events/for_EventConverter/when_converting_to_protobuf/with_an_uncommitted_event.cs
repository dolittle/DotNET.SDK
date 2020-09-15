// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.SDK.Events.for_EventConverter.when_converting_to_protobuf
{
    public class with_an_uncommitted_event : given.an_uncommitted_event
    {

        Establish context = () =>
        {

        };

        Because of = () => result = converter.ToProtobuf(uncommitted_event);

        It should_have_the_correct_artifact = () => result.
    }
}
