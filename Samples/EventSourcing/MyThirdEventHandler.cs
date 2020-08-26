// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Events;
using Dolittle.Events.Handling;
using Dolittle.Logging;

namespace EventSourcing
{
    [EventHandler("b2e8bc27-37ca-4857-9725-1ec6c0fc4d19")]
    public class MyThirdEventHandler : ICanHandleEvents
    {
        readonly ILogger _logger;

        public MyThirdEventHandler(ILogger logger)
        {
            _logger = logger;
        }

        public Task Handle(MyEvent @event, EventContext eventContext)
        {
            _logger.Information("Processing event : '{Event}'", @event);
            return Task.CompletedTask;
        }

        public Task Handle(MySecondEvent @event, EventContext eventContext)
        {
            _logger.Information("Processing event : '{Event}'", @event);
            return Task.CompletedTask;
        }
    }
}