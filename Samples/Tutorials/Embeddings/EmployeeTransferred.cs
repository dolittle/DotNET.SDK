// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events;

namespace Kitchen
{
    [EventType("b27f2a39-a2d4-43a7-9952-62e39cbc7ebc")]
    public class EmployeeTransferred
    {
        public string Name { get; set; }
        public string From { get; set; }
        public string To { get; set; }

        public EmployeeTransferred(string name, string from, string to)
        {
            Name = name;
            From = from;
            To = to;
        }
    }
}
