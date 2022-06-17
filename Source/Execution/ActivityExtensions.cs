// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


#nullable enable
using System.Diagnostics;
using Diagnostics;

namespace Dolittle.SDK.Execution;

public static class ActivityExtensions
{
    /// <summary>
    /// Set parent execution context if spanId is present
    /// </summary>
    /// <param name="activity"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public static Activity SetParentExecutionContext(this Activity activity, ExecutionContext context)
    {
        if (context.SpanId is not null)
        {
            activity.SetParentId(context.CorrelationId.ToActivityTraceId(), context.SpanId.Value);
        }

        return activity;
    }

    /// <summary>
    /// Starts a new activity, with this execution context as parent
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static Activity? StartChildActivity(this ExecutionContext context, string name, ActivityKind activityKind = ActivityKind.Internal) =>
        Tracing.ActivitySource.StartActivity(name, activityKind, context.ToActivityContext());

    /// <summary>
    /// Extracts the activity context if spanId is populated
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static ActivityContext ToActivityContext(this ExecutionContext context) =>
        context.SpanId is null
            ? default
            : new ActivityContext(context.CorrelationId.ToActivityTraceId(), context.SpanId.Value, ActivityTraceFlags.Recorded);

    static ActivityTraceId ToActivityTraceId(this CorrelationId correlationId)
    {
        return ActivityTraceId.CreateFromBytes(correlationId.Value.ToByteArray());
    }
}
