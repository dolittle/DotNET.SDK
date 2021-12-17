// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Common;
using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.DependencyInversion;

namespace Dolittle.SDK.Events.Handling.Builder;

/// <summary>
/// Defines a system that can build <see cref="IUniqueBindings{TIdentifier,TValue}"/> binding <see cref="EventHandlerId"/> to <see cref="IEventHandler"/>.
/// </summary>
public interface ICanBuildEventHandlerBindings
{
    /// <summary>
    /// Build the <see cref="IUniqueBindings{TIdentifier,TValue}"/> binding <see cref="EventHandlerId"/> to <see cref="IEventHandler"/> bindings.
    /// </summary>
    /// <param name="eventTypes">The <see cref="IEventTypes"/>.</param>
    /// <param name="buildResults">The <see cref="IClientBuildResults"/>.</param>
    /// <param name="tenantScopedProvidersFactory">The <see cref="Func{TResult}"/> getting the <see cref="ITenantScopedProviders"/>.</param>
    /// <returns>The <see cref="IUniqueBindings{TIdentifier,TValue}"/> binding <see cref="EventHandlerId"/> to <see cref="IEventHandler"/> bindings.</returns>
    IUniqueBindings<EventHandlerId, IEventHandler> Build(IEventTypes eventTypes, IClientBuildResults buildResults, Func<ITenantScopedProviders> tenantScopedProvidersFactory);
}
