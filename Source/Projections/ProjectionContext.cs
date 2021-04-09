// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Concepts;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Projections
{
    /// <summary>
    /// Represents the context of a projection.
    /// </summary>
    public class ProjectionContext : Value<ProjectionContext>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectionContext"/> class.
        /// </summary>
        /// <param name="wasCreatedFromInitialState">A value indicating whether the projection was created from the initial state or retrieved from a persisted state.</param>
        /// <param name="key">The projection <see cref="Key"/>.</param>
        /// <param name="eventContext">The <see cref="EventContext"/> in which the event occurred.</param>
        public ProjectionContext(bool wasCreatedFromInitialState, Key key, EventContext eventContext)
        {
            WasCreatedFromInitialState = wasCreatedFromInitialState;
            Key = key;
            EventContext = eventContext;
        }

        /// <summary>
        /// Gets a value indicating whether the projection was created from the initial state or retrieved from a persisted state.
        /// </summary>
        public bool WasCreatedFromInitialState { get; }

        /// <summary>
        /// Gets the projection <see cref="Key"/>.
        /// </summary>
        public Key Key { get; }

        /// <summary>
        /// Gets the <see cref="EventContext"/> in which the event occurred.
        /// </summary>
        public EventContext EventContext { get; }
    }
}