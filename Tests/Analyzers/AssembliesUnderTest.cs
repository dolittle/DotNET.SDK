// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using Dolittle.SDK.Aggregates;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Handling;
using Dolittle.SDK.Projections;
using Dolittle.SDK.Tenancy;

namespace Dolittle.SDK.Analyzers;

static class AssembliesUnderTest
{
    public static readonly Assembly[] Assemblies =
    {
        typeof(AggregateRootAttribute).Assembly,
        typeof(EventTypeAttribute).Assembly,
        typeof(ProjectionAttribute).Assembly,
        typeof(EventHandlerAttribute).Assembly,
        typeof(Tenant).Assembly,
        typeof(IDolittleClient).Assembly
    };
}
