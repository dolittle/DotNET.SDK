// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Projections.Builder;

/// <summary>
/// Defines a system that can build and register a projection.
/// </summary>
public interface ICanTryBuildProjection : IEquatable<ICanTryBuildProjection>
{
    /// <summary>
    /// Builds the projection.
    /// </summary>
    /// <param name="identifier">The <see cref="ProjectionModelId"/>.</param>
    /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
    /// <param name="buildResults">The <see cref="IClientBuildResults"/>.</param>
    /// <param name="projection">The built <see cref="IProjection"/>.</param>
    bool TryBuild(ProjectionModelId identifier, IEventTypes eventTypes, IClientBuildResults buildResults, [NotNullWhen(true)] out IProjection? projection);
}
