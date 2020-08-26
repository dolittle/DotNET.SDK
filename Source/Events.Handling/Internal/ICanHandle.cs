// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Events.Handling.Internal
{
    /// <summary>
    /// Defines an event handler that can handle events of type <typeparamref name="TEventType"/>.
    /// </summary>
    /// <typeparam name="TEventType">The event type that the handler can handle.</typeparam>
    public interface ICanHandle<TEventType>
        where TEventType : IEvent
    {
    }
}