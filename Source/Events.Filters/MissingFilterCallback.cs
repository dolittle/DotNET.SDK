// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Events.Filters
{
    /// <summary>
    /// Exception that gets thrown when a filter callback is not defines.
    /// </summary>
    public class MissingFilterCallback : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingFilterCallback"/> class.
        /// </summary>
        /// <param name="filterId">The <see cref="FilterId" />.</param>
        /// <param name="scopeId">The <see cref="ScopeId" />.</param>
        public MissingFilterCallback(FilterId filterId, ScopeId scopeId)
            : base($"Filter callback is not configured for filter '{filterId}' in scope '{scopeId}'")
        {
        }
    }
}