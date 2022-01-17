// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Aggregates.given;

[AggregateRoot("bab0924f-0dfc-4d79-8bab-ace680d6648c")]
public class StatefulAggregateRoot : AggregateRoot
{
    public StatefulAggregateRoot(EventSourceId eventSource)
        : base(eventSource)
    {
        AnEventOnCalled = 0;
        AnotherEventOnCalled = 0;
    }

    public int AnEventOnCalled { get; private set; }

    public int AnotherEventOnCalled { get; private set; }

    public MethodInfo OnSimpleEventMethod => GetMethodInfoFromCustomPointer("e9a77fc2-72ba-40e9-be2c-39b7198d0de7");

    public MethodInfo OnAnotherEventMethod => GetMethodInfoFromCustomPointer("1900ae56-ad66-4a80-936a-016574b298d5");

#pragma warning disable IDE0051, IDE0060
    [CustomMethodPointer("e9a77fc2-72ba-40e9-be2c-39b7198d0de7")]
    void On(AnEvent @event)
    {
        AnEventOnCalled++;
    }
#pragma warning restore IDE0051, IDE0060

#pragma warning disable IDE0051, IDE0060
    [CustomMethodPointer("1900ae56-ad66-4a80-936a-016574b298d5")]
    void On(AnotherEvent @event)
    {
        AnotherEventOnCalled++;
    }
#pragma warning restore IDE0051, IDE0060

    MethodInfo GetMethodInfoFromCustomPointer(string id)
    {
        foreach (var method in typeof(StatefulAggregateRoot).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance))
        {
            var pointer = method.GetCustomAttribute<CustomMethodPointerAttribute>();
            if (pointer != null && pointer.Id == id)
            {
                return method;
            }
        }

        return null;
    }
}