// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Events.Filters
{
    /// <summary>
    /// Represents the result of a <see cref="ICanFilterEvents"/>.
    /// </summary>
    public class FilterResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterResult"/> class.
        /// </summary>
        /// <param name="included">true if the <see cref="IEvent"/> should be included in the stream, false if not.</param>
        public FilterResult(bool included)
        {
            Included = included;
        }

        /// <summary>
        /// Gets a value indicating whether or not the <see cref="IEvent"/> should be included in the stream.
        /// </summary>
        public bool Included { get; }

        /// <summary>
        /// Implicitly convert from a <see cref="bool"/> to <see cref="FilterResult"/>.
        /// </summary>
        /// <param name="included">true if the <see cref="IEvent"/> should be included in the stream, false if not.</param>
        public static implicit operator FilterResult(bool included) => new FilterResult(included);
    }
}