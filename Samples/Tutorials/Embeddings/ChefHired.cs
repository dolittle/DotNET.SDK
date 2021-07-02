// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events;

namespace Kitchen
{
    [EventType("a7a499a8-816f-4bc5-96cc-a44a6f5d1b04")]
    public class ChefHired
    {
        public ChefHired(string chef)
        {
            Chef = chef;
        }

        public string Chef { get; }
    }
}
