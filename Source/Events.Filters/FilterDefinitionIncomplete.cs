// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Events.Filters;

/// <summary>
/// Exception that gets thrown when a filter definition is started using the <see cref="EventFiltersBuilder" />, but not completed.
/// </summary>
public class FilterDefinitionIncomplete : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FilterDefinitionIncomplete"/> class.
    /// </summary>
    /// <param name="filterId">The <see cref="FilterId" />.</param>
    /// <param name="scopeId">The <see cref="ScopeId" />.</param>
    /// <param name="action">The action to do for the filter definition to not be incomplete anymore.</param>
    public FilterDefinitionIncomplete(FilterId filterId, ScopeId scopeId, string action)
        : base($"Filter definition for filter '{filterId}' in scope '{scopeId}' is incomplete: ${action}")
    {
    }
}