// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Failures.Events.Handling
{
    /// <summary>
    /// Exception that gets thrown when no event handler registration arguments is recevied by the runtime.
    /// </summary>
    public class NoEventHandlerRegistrationReceived : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoEventHandlerRegistrationReceived"/> class.
        /// </summary>
        /// <param name="reason">The failure reason.</param>
        public NoEventHandlerRegistrationReceived(string reason)
            : base(reason)
        {
        }
    }
}