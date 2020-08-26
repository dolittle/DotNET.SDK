// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Events.Handling.Internal;

namespace Dolittle.Events.Handling.EventHorizon
{
    /// <summary>
    /// Defines a system that can provide <see cref="ICanHandleExternalEvents"/> implementations.
    /// </summary>
    public interface ICanProvideExternalEventHandlers : ICanProvideHandlers<ICanHandleExternalEvents, IPublicEvent>
    {
    }
}