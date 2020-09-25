// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;

namespace Dolittle.SDK.Events.Handling.Builder
{
    /// <summary>
    /// Represents the build warnings from when an event handler was attempted to be built.
    /// </summary>
    public class EventHandlerBuildWarnings
    {
        readonly EventHandlerId _eventHandlerId;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlerBuildWarnings"/> class.
        /// </summary>
        /// <param name="eventHandlerId">The <see cref="EventHandlerId" />.</param>
        /// <param name="warnings">The <see cref="IEnumerable{T}" /> of warning messages.</param>
        public EventHandlerBuildWarnings(EventHandlerId eventHandlerId, IEnumerable<string> warnings)
        {
            _eventHandlerId = eventHandlerId;
            Warnings = warnings;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlerBuildWarnings"/> class.
        /// </summary>
        /// <param name="eventHandlerId">The <see cref="EventHandlerId" />.</param>
        /// <param name="warnings">The <see cref="IEnumerable{T}" /> of warning messages.</param>
        public EventHandlerBuildWarnings(EventHandlerId eventHandlerId, params string[] warnings)
            : this(eventHandlerId, warnings.ToList())
        {
        }

        /// <summary>
        /// Gets the <see cref="IEnumerable{T}" /> of warnings when attempting to build the event handler.
        /// </summary>
        public IEnumerable<string> Warnings { get; }

        /// <inheritdoc/>
        public override string ToString()
            => $"Failed to build event handler {_eventHandlerId}{System.Environment.NewLine}{string.Join($"{System.Environment.NewLine}\t", Warnings)}";
    }
}
