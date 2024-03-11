// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Projections.Types;

[EventType("83225425-c4b8-4ef7-9638-d23530831752")]
public record NameChanged(string Name);

[AggregateRoot("d909c8a4-d21f-46ab-a97a-a34d40697a0e")]
public class TestAggregate : AggregateRoot
{
    string? _name;

    public void Rename(string name)
    {
        if (string.IsNullOrEmpty(name)) throw new ArgumentException("Name cannot be empty");

        if (name.Equals(_name))
        {
            return;
        }

        Apply(new NameChanged(name));
    }

    void On(NameChanged evt)
    {
        _name = evt.Name;
    }
}

[Projection("703c7d67-179e-408d-b537-1c0ab4f8e250")]
public class AggregateProjection : ReadModel
{
    public string Name { get; set; } = string.Empty;
    public int TimesChanged { get; set; }

    public void On(NameChanged evt)
    {
        Name = evt.Name;
        TimesChanged++;
    }
}
