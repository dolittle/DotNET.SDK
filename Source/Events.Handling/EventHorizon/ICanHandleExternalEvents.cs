// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Events.Handling.Internal;

namespace Dolittle.Events.Handling.EventHorizon
{
    /// <summary>
    /// Defines a system that can handle <see cref="IPublicEvent" /> events from other microservices.
    /// </summary>
    /// <remarks>
    /// An implementation must then implement Handle methods that takes the
    /// specific <see cref="IPublicEvent">events</see> you want to handle.
    /// </remarks>
    public interface ICanHandleExternalEvents : ICanHandle<IPublicEvent>
    {
    }
}