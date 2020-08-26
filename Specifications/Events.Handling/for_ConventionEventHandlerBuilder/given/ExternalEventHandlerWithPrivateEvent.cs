// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Events.Handling.EventHorizon;

namespace Dolittle.Events.Handling.for_ConventionEventHandlerBuilder.given
{
    public class ExternalEventHandlerWithPrivateEvent : ICanHandleExternalEvents
    {
        public Task Handle(MyFirstEvent @event, EventContext context)
            => Task.CompletedTask;
    }
}