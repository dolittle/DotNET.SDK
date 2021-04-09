// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events;

namespace Dolittle.SDK.Projections.Builder
{
    /// <summary>
    /// Represents a builder for building the key selector expression for projection's on() method.
    /// </summary>
    public class KeySelectorBuilder
    {
        /// <summary>
        /// Select projection key from the <see cref="EventSourceId"/>.
        /// </summary>
        /// <returns>A <see cref="KeySelector"/>.</returns>
        public KeySelector KeyFromEventSource()
            => new KeySelector(KeySelectorType.EventSourceId);

        /// <summary>
        /// Select projection key from the <see cref="PartitionId"/>.
        /// </summary>
        /// <returns>A <see cref="KeySelector"/>.</returns>
        public KeySelector KeyFromPartitionId()
            => new KeySelector(KeySelectorType.PartitionId);

        /// <summary>
        /// Select projection key from a property of the event.
        /// </summary>
        /// <param name="property">The property on the event.</param>
        /// <returns>A <see cref="KeySelector"/>.</returns>
        public KeySelector KeyFromProperty(string property)
            => new KeySelector(KeySelectorType.Property, property);
    }
}
