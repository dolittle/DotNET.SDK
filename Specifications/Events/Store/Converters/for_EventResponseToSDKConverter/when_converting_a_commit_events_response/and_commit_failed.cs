// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Protobuf.Contracts;
using Dolittle.Runtime.Events.Contracts;
using Dolittle.SDK.Failures.Events;
using Dolittle.SDK.Protobuf;
using Machine.Specifications;

namespace Dolittle.SDK.Events.Store.Converters.for_EventResponseToSDKConverter.when_converting_a_commit_events_response
{
    public class and_commit_failed : given.a_converter_and_a_protobuf_execution_context
    {
        static Uuid failure_id;
        static string failure_reason;
        static CommitEventsResponse commit_events_response;
        static CommittedEvents committed_events;
        static Exception exception;
        static bool try_result;

        Establish context = () =>
        {
            // failure for EventStorePersistenceError
            failure_id = Guid.Parse("ad55fca7-476a-4f68-9411-1a3b087ab843").ToProtobuf();
            failure_reason = "out of tacos";
            commit_events_response = new CommitEventsResponse
            {
                Failure = new Failure
                {
                        Id = failure_id,
                        Reason = failure_reason
                },
                Events = { }
            };
        };

        Because of = () => try_result = converter.TryToSDK(commit_events_response, out committed_events, out exception);

        It should_return_false = () => try_result.ShouldBeFalse();
        It should_out_default_events = () => committed_events.ShouldEqual(default);
        It should_out_event_store_persistence_error = () => exception.ShouldBeOfExactType<EventStorePersistenceError>();
    }
}
