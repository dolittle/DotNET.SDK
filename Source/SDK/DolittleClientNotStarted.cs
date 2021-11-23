// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK
{
    /// <summary>
    /// Exception that gets thrown when the trying to access properties on the <see cref="IDolittleClient"/> that requires it to be started.
    /// </summary>
    public class DolittleClientNotStarted : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DolittleClientNotStarted"/> class.
        /// </summary>
        public DolittleClientNotStarted()
            : base($"{nameof(IDolittleClient)} has not been started yet. Invoke the {nameof(IDolittleClient.Start)}() method")
        {
        }
    }
}