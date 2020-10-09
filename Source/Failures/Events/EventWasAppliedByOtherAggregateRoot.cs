// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Failures.Events
{
    /// <summary>
    /// Exception that gets thrown when an event is being used with an Aggregate Root with a different Aggregate Root than it was applied by.
    /// </summary>
    public class EventWasAppliedByOtherAggregateRoot : ArgumentException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventWasAppliedByOtherAggregateRoot"/> class.
        /// </summary>
        /// <param name="reason">The failure reason.</param>
        public EventWasAppliedByOtherAggregateRoot(string reason)
            : base(reason)
        {
        }
    }
}
