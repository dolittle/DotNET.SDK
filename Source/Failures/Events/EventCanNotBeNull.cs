// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Failures.Events
{
    /// <summary>
    /// Exception that gets thrown when an event is null.
    /// </summary>
    public class EventCanNotBeNull : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventCanNotBeNull"/> class.
        /// </summary>
        /// <param name="reason">The failure reason.</param>
        public EventCanNotBeNull(string reason)
            : base(reason)
        {
        }
    }
}