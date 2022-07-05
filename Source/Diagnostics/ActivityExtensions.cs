// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using OpenTelemetry.Trace;

namespace Diagnostics;

/// <summary>
/// System.Diagnostics.Activity extensions
/// </summary>
public static class ActivityExtensions
{
    public static void RecordError(this Activity activity, Exception e)
    {
        activity
            .SetStatus(ActivityStatusCode.Error)
            .RecordException(e);
    }
}
