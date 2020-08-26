// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Events;

namespace EventSourcing
{
    public class MySecondEvent : IEvent
    {
        public MySecondEvent(string stuff)
        {
            Stuff = stuff;
        }

        public string Stuff { get; }
    }
}