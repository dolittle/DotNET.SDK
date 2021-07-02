// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events;

namespace Kitchen
{
    [EventType("991c808e-e3bb-4b3c-873a-4d4acb7c887c")]
    public class ChefFired
    {
        public ChefFired(string chef)
        {
            Chef = chef;
        }

        public string Chef { get; }
    }
}
