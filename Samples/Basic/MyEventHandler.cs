// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Handling;

namespace Basic
{
    [EventHandler("98475832-0405-4169-8e25-85a6988c7fc7")]
    class MyEventHandler
    {
        [Handles("23ca92f3-22c3-44a7-be7b-64364b377eec")]
        public void Handle(MyEvent @event, EventContext context)
        {
            Console.WriteLine("Handling event in event handler class");
        }
    }
}
