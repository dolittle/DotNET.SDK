// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Events.Filters;

/// <summary>
/// Exception that gets thrown when a filter callback is not defines.
/// </summary>
public class MissingFilterCallback : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MissingFilterCallback"/> class.
    /// </summary>
    /// <param name="filterId">The <see cref="FilterId" />.</param>
    public MissingFilterCallback(FilterModelId filterId)
        : base($"Filter callback is not configured for filter '{filterId.Id}' in scope '{filterId.Scope}'")
    {
    }
}
