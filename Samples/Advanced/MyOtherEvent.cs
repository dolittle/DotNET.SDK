// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events;

namespace Basic
{
    [EventType("2fc3df44-0ee6-41d7-b0eb-eb2b841894df")]
    class MyOtherEvent
    {
        public MyOtherEvent(string aString, int anInteger)
        {
            AString = aString;
            AnInteger = anInteger;
        }
        public string AString { get; }
        public int AnInteger { get; }
    }
}
