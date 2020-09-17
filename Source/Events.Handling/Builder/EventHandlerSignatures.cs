// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;

namespace Dolittle.SDK.Events.Handling.Builder
{
    /// <summary>
    /// Represents the signature for an event handler.
    /// </summary>
    /// <param name="event">The event to handle.</param>
    /// <param name="eventContext">The <see cref="EventContext" />.</param>
    public delegate Task EventHandlerSignature(object @event, EventContext eventContext);

    /// <summary>
    /// Represents the signature for an event handler.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type" /> of the event to handle.</typeparam>
    /// <param name="event">The event to handle.</param>
    /// <param name="eventContext">The <see cref="EventContext" />.</param>
    public delegate Task TypedEventHandlerSignature<T>(T @event, EventContext eventContext)
        where T : class;
}