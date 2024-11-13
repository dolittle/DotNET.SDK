// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;

namespace Dolittle.SDK.Diagnostics;

/// <summary>
/// System.Diagnostics.Activity extensions
/// </summary>
public static class ActivityExtensions
{
    /// <summary>
    /// Record an exception
    /// Sets status to error and adds the exception to the activity.
    /// </summary>
    /// <param name="activity"></param>
    /// <param name="e">The Exception</param>
    public static void RecordError(this Activity activity, Exception e)
    {
        activity
            .SetStatus(ActivityStatusCode.Error)
            .AddException(e);
    }
}
