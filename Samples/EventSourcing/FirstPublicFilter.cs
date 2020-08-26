// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Events;
using Dolittle.Events.Filters;
using Dolittle.Events.Filters.EventHorizon;
using Dolittle.Logging;

namespace EventSourcing
{
    [Filter("db7acf1e-a640-41f3-934f-f2f188e1b1e9")]
    public class FirstPublicFilter : ICanFilterPublicEvents
    {
        readonly ILogger _logger;

        public FirstPublicFilter(ILogger logger)
        {
            _logger = logger;
        }

        public Task<FilterResult> Filter(CommittedEvent @event)
        {
            _logger.Warning("Filtering public event '{Event}'", @event);
            return Task.FromResult(new FilterResult(true));
        }
    }
}
