// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Events.Handling.Internal;

namespace Dolittle.Events.Handling
{
    /// <summary>
    /// Defines an event subscriber.
    /// </summary>
    /// <remarks>
    /// An implementation must then implement a Handle method that takes the
    /// specific <see cref="IEvent">event</see> you want to handle.
    /// </remarks>
    public interface ICanHandleEvents : ICanHandle<IEvent>
    {
    }
}
