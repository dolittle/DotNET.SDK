// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK
{
    /// <summary>
    /// Exception that gets thrown when calling <see cref="IDolittleClient.Start()"/> without it being connected to a Runtime.
    /// </summary>
    public class CannotStartUnconnectedDolittleClient : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CannotStartUnconnectedDolittleClient"/> class.
        /// </summary>
        public CannotStartUnconnectedDolittleClient()
            : base($"Cannot call {nameof(IDolittleClient)}.{nameof(IDolittleClient.Start)}() before connecting it to the Runtime.")
        {
        }
    }
}