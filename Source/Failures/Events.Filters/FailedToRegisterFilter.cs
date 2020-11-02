// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Failures.Events.Filters
{
    /// <summary>
    /// Exception that gets thrown when the runtime could not register a filter.
    /// </summary>
    public class FailedToRegisterFilter : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FailedToRegisterFilter"/> class.
        /// </summary>
        /// <param name="reason">The failure reason.</param>
        public FailedToRegisterFilter(string reason)
            : base(reason)
        {
        }
    }
}