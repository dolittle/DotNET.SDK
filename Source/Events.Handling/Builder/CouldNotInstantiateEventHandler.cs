// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.DependencyInversion;

namespace Dolittle.SDK.Events.Handling.Builder
{
    /// <summary>
    /// Exception that gets thrown when <see cref="IContainer" /> could not instantitate event handler.
    /// </summary>
    public class CouldNotInstantiateEventHandler : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CouldNotInstantiateEventHandler"/> class.
        /// </summary>
        /// <param name="eventHandlerType">The <see cref="Type" /> of the event handler.</param>
        public CouldNotInstantiateEventHandler(Type eventHandlerType)
            : base($"{eventHandlerType} could not be instantiated by IoC container.")
        {
        }
    }
}
