// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Linq;
using Dolittle.SDK.Events.Store;

namespace Dolittle.SDK.Events;

/// <summary>
/// System.Diagnostics.Activity extensions for events
/// </summary>
public static class ActivityExtensions
{
    const string EventType = "type.id";
    const string EventTypeAlias = "type.alias";
    const string EventSourceId = "eventsource.id";

    /// <param name="activity">Activity to tag</param>
    /// <param name="eventType">EventType metadata</param>
    /// <returns></returns>
    public static Activity Tag(this Activity activity, EventType eventType)
    {
        activity.SetTag(EventType, eventType.Id.Value);
        if (eventType.HasAlias)
        {
            activity.SetTag(EventTypeAlias, eventType.Alias.Value);
        }

        return activity;
    }

    /// <param name="activity">Activity to tag</param>
    /// <param name="eventSourceId">EventSourceId metadata</param>
    /// <returns></returns>
    public static Activity Tag(this Activity activity, EventSourceId eventSourceId)
    {
        activity.SetTag(EventSourceId, eventSourceId.Value);

        return activity;
    }

    /// <param name="activity">Activity to tag</param>
    /// <param name="uncommittedEvents">EventSourceId metadata</param>
    /// <returns></returns>
    public static Activity Tag(this Activity activity, UncommittedEvents uncommittedEvents)
    {
        foreach (var grouping in uncommittedEvents.GroupBy(@event => @event.EventType))
        {
            activity.SetTag($"{grouping.Key.Id}.count", grouping.Count());
        }

        return activity;
    }
}
