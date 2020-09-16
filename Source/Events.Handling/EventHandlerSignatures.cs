// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;

namespace Dolittle.SDK.Events.Handling
{
    /// <summary>
    /// Represents the signature for an event handler.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type" /> of the event.</typeparam>
    /// <param name="event">The event to handle.</param>
    /// <param name="eventContext">The <see cref="EventContext" />.</param>
    public delegate Task EventHandlerSignature<T>(T @event, EventContext eventContext)
        where T : class;
}