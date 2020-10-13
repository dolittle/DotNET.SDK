// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Events.Store
{
    /// <summary>
    /// Exception that gets thrown when trying to construct an <see cref="CommittedExternalEvent"/> without an external <see cref="EventLogSequenceNumber"/>.
    /// </summary>
    public class ExternalEventLogSequenceNumberCannotBeNull : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalEventLogSequenceNumberCannotBeNull"/> class.
        /// </summary>
        public ExternalEventLogSequenceNumberCannotBeNull()
            : base($"The external {nameof(EventLogSequenceNumber)} of an {nameof(CommittedExternalEvent)} cannot be null")
        {
        }
    }
}