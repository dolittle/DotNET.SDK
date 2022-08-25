// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.ApplicationModel.ClientSetup;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Embeddings.Builder;

/// <summary>
/// Defines a system that can build and register a projection.
/// </summary>
public interface ICanTryBuildEmbedding : IEquatable<ICanTryBuildEmbedding>
{
    /// <summary>
    /// Builds and registers the projection.
    /// </summary>
    /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
    /// <param name="buildResults">The <see cref="IClientBuildResults"/>.</param>
    /// <param name="embedding">The built <see cref="IEmbedding"/>.</param>
    bool TryBuild(IEventTypes eventTypes, IClientBuildResults buildResults, out Internal.IEmbedding embedding);
}
