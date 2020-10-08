// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Failures.Events.Filters
{
    /// <summary>
    /// Exception that gets thrown when no filter registration arguments is recevied by the runtime.
    /// </summary>
    public class NoFilterRegistrationReceived : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoFilterRegistrationReceived"/> class.
        /// </summary>
        /// <param name="reason">The failure reason.</param>
        public NoFilterRegistrationReceived(string reason)
            : base(reason)
        {
        }
    }
}