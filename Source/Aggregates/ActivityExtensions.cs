// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Aggregates;

/// <summary>
/// System.Diagnostics.Activity extensions for aggregates
/// </summary>
public static class ActivityExtensions
{
    const string AggregateRootId = "aggregate.id";
    
    /// <summary>
    /// Set parent execution context if spanId is present
    /// </summary>
    /// <param name="activity">Activity to tag</param>
    /// <param name="aggregateRootId">Aggregate root id</param>
    /// <returns></returns>
    public static Activity Tag(this Activity activity, AggregateRootId aggregateRootId)
    {
        activity.SetTag(AggregateRootId, aggregateRootId.Value);

        return activity;
    }
}
