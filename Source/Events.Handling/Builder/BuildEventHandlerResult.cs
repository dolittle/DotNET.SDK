// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;

namespace Dolittle.SDK.Events.Handling.Builder
{
    /// <summary>
    /// Represents the result of building an event handler.
    /// </summary>
    public class BuildEventHandlerResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildEventHandlerResult"/> class.
        /// </summary>
        /// <param name="warnings">The optional <see cref="EventHandlerBuildWarnings" />.</param>
        public BuildEventHandlerResult(EventHandlerBuildWarnings warnings = default)
            => Warnings = warnings;

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildEventHandlerResult"/> class.
        /// </summary>
        /// <param name="eventHandlerId">The <see cref="EventHandlerId" />.</param>
        /// <param name="warnings">The warnings.</param>
        public BuildEventHandlerResult(EventHandlerId eventHandlerId, IEnumerable<string> warnings)
            => Warnings = warnings.Any() ? new EventHandlerBuildWarnings(eventHandlerId, warnings) : default;

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildEventHandlerResult"/> class.
        /// </summary>
        /// <param name="eventHandlerId">The <see cref="EventHandlerId" />.</param>
        /// <param name="warnings">The warnings.</param>
        public BuildEventHandlerResult(EventHandlerId eventHandlerId, params string[] warnings)
            : this(eventHandlerId, warnings.ToList())
        {
        }

        /// <summary>
        /// Gets a value indicating whether building the event handler succeeded.
        /// </summary>
        public bool Succeeded => Warnings == default;

        /// <summary>
        /// Gets the <see cref="EventHandlerBuildWarnings" /> - null if no warnings and <see cref="Succeeded" />.
        /// </summary>
        public EventHandlerBuildWarnings Warnings { get; }

        /// <summary>
        /// Implicitly converts <see cref="BuildEventHandlerResult" /> to <see cref="bool" />.
        /// </summary>
        /// <param name="result">Whether <see cref="Succeeded" />.</param>
        public static implicit operator bool(BuildEventHandlerResult result) => result.Succeeded;
    }
}
