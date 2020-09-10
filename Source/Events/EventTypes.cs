// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Artifacts;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Events
{
    /// <summary>
    /// Represents a system that knows about<see cref="EventType" />.
    /// </summary>
    public class EventTypes : Artifacts<EventType>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventTypes"/> class.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public EventTypes(ILogger logger)
            : base(logger)
        {
        }
    }
}
