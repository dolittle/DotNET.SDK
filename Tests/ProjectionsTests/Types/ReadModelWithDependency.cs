// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.SDK.Projections.Types;

public delegate void TestDependency(NameChanged evt);

[Projection("bad319f1-6af8-48a0-b190-323e21ba6cde")]
public class ReadModelWithDependency : ReadModel, IRequireDependencies<ReadModelWithDependency>
{
    TestDependency? _dep;

    public string Name { get; set; } = string.Empty;
    public int TimesChanged { get; set; }


    public void Resolve(IServiceProvider serviceProvider) => _dep = serviceProvider.GetRequiredService<TestDependency>();

    public void On(NameChanged evt)
    {
        Name = evt.Name;
        TimesChanged++;
        _dep?.Invoke(evt);
    }
}
