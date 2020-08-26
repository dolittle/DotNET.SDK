// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Dolittle.Logging;
using Dolittle.Types;

namespace Dolittle.Events.Handling.EventHorizon
{
    /// <summary>
    /// An implementation of <see cref="ICanProvideExternalEventHandlers"/>.
    /// </summary>
    public class ExternalEventHandlersProvider : ICanProvideExternalEventHandlers
    {
        readonly IImplementationsOf<ICanHandleExternalEvents> _handlers;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalEventHandlersProvider"/> class.
        /// </summary>
        /// <param name="handlers"><see cref="IInstancesOf{T}"/> of type <see cref="ICanHandleExternalEvents"/>.</param>
        /// <param name="logger">The <see cref="ILogger"/> to use for logging.</param>
        public ExternalEventHandlersProvider(IImplementationsOf<ICanHandleExternalEvents> handlers, ILogger logger)
        {
            _handlers = handlers;
            _logger = logger;
        }

        /// <inheritdoc/>
        public IImplementationsOf<ICanHandleExternalEvents> Provide()
        {
            _logger.Debug("Providing {EventHandlerCount} external event handlers", _handlers.Count());
            return _handlers;
        }
    }
}