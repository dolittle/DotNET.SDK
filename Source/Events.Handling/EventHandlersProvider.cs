// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Dolittle.Logging;
using Dolittle.Types;

namespace Dolittle.Events.Handling
{
    /// <summary>
    /// An implementation of <see cref="ICanProvideEventHandlers"/>.
    /// </summary>
    public class EventHandlersProvider : ICanProvideEventHandlers
    {
        readonly IImplementationsOf<ICanHandleEvents> _handlers;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlersProvider"/> class.
        /// </summary>
        /// <param name="handlers"><see cref="IImplementationsOf{T}"/> of type <see cref="ICanHandleEvents"/>.</param>
        /// <param name="logger">The <see cref="ILogger"/> to use for logging.</param>
        public EventHandlersProvider(IImplementationsOf<ICanHandleEvents> handlers, ILogger logger)
        {
            _handlers = handlers;
            _logger = logger;
        }

        /// <inheritdoc/>
        public IImplementationsOf<ICanHandleEvents> Provide()
        {
            _logger.Debug("Providing {EventHandlerCount} event handlers", _handlers.Count());
            return _handlers;
        }
    }
}