// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Services
{
    /// <summary>
    /// Exception that gets thrown when a processor is added to an already stopped processing coordinator.
    /// </summary>
    public class CannotAddProcessorsToAStoppedProcessingCoordinator : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CannotAddProcessorsToAStoppedProcessingCoordinator"/> class.
        /// </summary>
        public CannotAddProcessorsToAStoppedProcessingCoordinator()
            : base("Cannot add a processor to an already stopped processing coordinator.")
            {
            }
    }
}
