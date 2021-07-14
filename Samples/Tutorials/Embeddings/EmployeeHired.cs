// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events;

namespace Kitchen
{
    [EventType("8fdf45bc-f484-4348-bcb0-4d6f134aaf6c")]
    public class EmployeeHired
    {
        public string Name { get; set; }

        public EmployeeHired(string name) => Name = name;
    }
}
