// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events;

[EventType("1932beb4-c8cd-4fee-9a7e-a92af3693510")]
public class EmployeeRetired
{
    public string Name { get; set; }

    public EmployeeRetired(string name) => Name = name;
}

