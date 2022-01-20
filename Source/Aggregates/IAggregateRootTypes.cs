// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Artifacts;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Aggregates;

/// <summary>
/// Defines a system that knows about <see cref="AggregateRootType" />.
/// </summary>
public interface IAggregateRootTypes : IArtifacts<AggregateRootType, AggregateRootId>
{
}
