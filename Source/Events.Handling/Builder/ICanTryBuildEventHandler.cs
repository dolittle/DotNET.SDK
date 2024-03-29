// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Common.ClientSetup;

namespace Dolittle.SDK.Events.Handling.Builder;

/// <summary>
/// Defines a builder than can build <see cref="IEventHandler"/>.
/// </summary>
public interface ICanTryBuildEventHandler
{
    /// <summary>
    /// Builds event handler.
    /// </summary>
    /// <param name="identifier">The <see cref="EventHandlerModelId"/>.</param>
    /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
    /// <param name="buildResults">The <see cref="IClientBuildResults"/>.</param>
    /// <param name="eventHandler">The built <see cref="IEventHandler"/>.</param>
    bool TryBuild(EventHandlerModelId identifier, IEventTypes eventTypes, IClientBuildResults buildResults, out IEventHandler eventHandler);
}
