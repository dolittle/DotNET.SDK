// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Events.Builders
{
    /// <summary>
    /// Exception that gets thrown when a method is called more than once while building an event subscription.
    /// </summary>
    public class EventBuilderMethodAlreadyCalled : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventBuilderMethodAlreadyCalled"/> class.
        /// </summary>
        /// <param name="method">The method that was called more than once.</param>
        public EventBuilderMethodAlreadyCalled(string method)
            : base($"The method {method} can only be called once while building an event.")
        {
        }
    }
}
