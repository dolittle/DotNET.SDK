// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;

namespace Dolittle.Events.Handling.for_ConventionEventHandlerBuilder.given
{
    public class EventHandlerWithCorrectPublicMethodSignatureButWrongName : ICanHandleEvents
    {
#pragma warning disable IDE0060
        public Task Handla(MyFirstEvent @event, EventContext context)
            => Task.CompletedTask;
#pragma warning restore IDE0060
    }
}