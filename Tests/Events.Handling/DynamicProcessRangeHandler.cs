// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Handling;

namespace Events.Handling;

[EventType("74267f8a-f474-4cad-9172-4094d0e0af1c")]
public class TestEvent
{
    public string Content { get; set; }
}

class TestSelector : IProcessRangeSelector<TestSelector>
{
    public static ProcessFrom ProcessFrom { get; set; } = ProcessFrom.Earliest;

    public static DateTimeOffset? From { get; set; }

    public static DateTimeOffset? To { get; set; }

    public ProcessRange GetRange() => new(ProcessFrom, From, To);
}

[EventHandlerWithRangeSelector<TestSelector>(Identifier)]
public class DynamicProcessRangeHandler
{
    public const string Identifier = "865dbc04-39ed-421c-9c70-db49f381f980";
    
    public Task Handle(TestEvent evt, EventContext ctx)
    {
        return Task.CompletedTask;
    }
}

[EventHandler(Identifier)]
public class OtherHandler
{
    public const string Identifier = "18cfe5a2-302e-44ae-af85-7b7b289fd10c";

    public Task Handle(TestEvent evt, EventContext ctx)
    {
        return Task.CompletedTask;
    }
}
