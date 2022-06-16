// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using Dolittle.Execution.Contracts;
using Dolittle.Protobuf.Contracts;
using Google.Protobuf;

namespace Dolittle.SDK.Services;

static class ActivityExtensions
{
    public static void SetTraceContext(this Activity activity, ExecutionContext context)
    {
        context.CorrelationId = activity.TraceId.ToUuid();
        context.SpanId = activity.SpanId.ToByteString();
    }
    
    static Uuid ToUuid(this ActivityTraceId traceId)
    {
        var bytes = new byte[16];
        traceId.CopyTo(bytes);
        return new Uuid
        {
            Value = ByteString.CopyFrom(bytes)
        };
    }
    
    static ByteString ToByteString(this ActivitySpanId spanId)
    {
        var bytes = new byte[8];
        spanId.CopyTo(bytes);
        return ByteString.CopyFrom(bytes);
    }
}
