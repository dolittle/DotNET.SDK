// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Dolittle.SDK.Artifacts;

namespace Dolittle.SDK.Events
{
    /// <summary>
    /// Represents an event that has not been committed to the Event Store.
    /// </summary>
    public class UncommittedEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UncommittedEvent"/> class.
        /// </summary>
        /// <param name="eventSource">The <see cref="EventSourceId" /> of the Event.</param>
        /// <param name="artifact"></param>
        /// <param name="content"></param>
        /// <param name="isPublic"></param>
        public UncommittedEvent(EventSourceId eventSource, Artifact artifact, object content, bool isPublic)
        {
            EventSource = eventSource;
            Artifact = artifact;
            Content = content;
            IsPublic = isPublic;
        }

        /// <summary>
        /// Gets the Event Source that this event was applied to.
        /// </summary>
        public EventSourceId EventSource { get; }

        /// <summary>
        /// Gets the Artifact this event is associated with.
        /// </summary>
        public Artifact Artifact { get; }

        /// <summary>
        /// Gets the content of the event.
        /// </summary>
        public object Content { get; }

        /// <summary>
        /// Gets whether the event is public or not.
        /// </summary>
        public bool IsPublic { get; }
    }
}
