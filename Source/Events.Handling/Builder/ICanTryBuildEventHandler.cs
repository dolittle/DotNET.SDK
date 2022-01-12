// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.DependencyInversion;

namespace Dolittle.SDK.Events.Handling.Builder;

/// <summary>
/// Defines a builder than can build <see cref="IEventHandler"/>.
/// </summary>
public interface ICanTryBuildEventHandler : IEquatable<ICanTryBuildEventHandler>
{
    /// <summary>
    /// Builds event handler.
    /// </summary>
    /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
    /// <param name="buildResults">The <see cref="IClientBuildResults"/>.</param>
    /// <param name="tenantScopedProvidersFactory">The <see cref="System.Func{TResult}"/> for getting the <see cref="ITenantScopedProviders"/>.</param>
    /// <param name="eventHandler">The built <see cref="IEventHandler"/>.</param>
    bool TryBuild(
        IEventTypes eventTypes,
        IClientBuildResults buildResults,
        System.Func<ITenantScopedProviders> tenantScopedProvidersFactory,
        out IEventHandler eventHandler);
}
