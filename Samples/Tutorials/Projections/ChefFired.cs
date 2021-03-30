// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Sample code for the tutorial at https://dolittle.io/tutorials/getting-started/projections/

using Dolittle.SDK.Events;

namespace Kitchen
{
    [EventType("bc6a3511-aea8-421a-b7da-4e8df30be966")]
    public class ChefFired
    {
        public string Chef;
    }
}
