// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events;

namespace Basic
{
    [EventType("f42529b3-d980-4b55-8fbe-65101a6141a3")]
    class MyEvent
    {
        public MyEvent(string aString, int anInteger)
        {
            AString = aString;
            AnInteger = anInteger;
        }
        public string AString { get; }
        public int AnInteger { get; }
    }
}
