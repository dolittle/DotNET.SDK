// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Basic
{
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
