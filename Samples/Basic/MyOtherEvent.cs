// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK;

namespace Basic
{
    class MyOtherEvent
    {
        public MyOtherEvent(string aString)
        {
            AString = aString;
        }
        public string AString { get; }
    }
}
