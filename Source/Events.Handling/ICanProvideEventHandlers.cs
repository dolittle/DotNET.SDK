// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Events.Handling.Internal;

namespace Dolittle.Events.Handling
{
    /// <summary>
    /// Defines a system that can provide <see cref="ICanHandleEvents"/> implementations.
    /// </summary>
    public interface ICanProvideEventHandlers : ICanProvideHandlers<ICanHandleEvents, IEvent>
    {
    }
}