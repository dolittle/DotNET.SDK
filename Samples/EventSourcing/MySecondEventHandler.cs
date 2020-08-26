// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Events;
using Dolittle.Events.Handling;
using Dolittle.Logging;

namespace EventSourcing
{
    [EventHandler("ac471b46-17e4-410d-ab9d-58b03edcab91")]
    public class MySecondEventHandler : ICanHandleEvents
    {
        readonly ILogger _logger;

        public MySecondEventHandler(ILogger logger)
        {
            _logger = logger;
        }

        public Task Handle(MyEvent @event, EventContext eventContext)
        {
            _logger.Information("Processing event : '{Event}'", @event);
            return Task.CompletedTask;
        }
    }
}