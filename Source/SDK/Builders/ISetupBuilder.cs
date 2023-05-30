// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Aggregates.Builders;
using Dolittle.SDK.EventHorizon;
using Dolittle.SDK.Events.Builders;
using Dolittle.SDK.Events.Filters.Builders;
using Dolittle.SDK.Events.Handling.Builder;
using Dolittle.SDK.Projections.Builder;

namespace Dolittle.SDK.Builders;

/// <summary>
/// Defines a builder for setting up a <see cref="DolittleClient"/>.
/// </summary>
public interface ISetupBuilder
{
    /// <summary>
    /// Sets the event types through the <see cref="IEventTypesBuilder" />.
    /// </summary>
    /// <param name="callback">The builder callback.</param>
    /// <returns>The client builder for continuation.</returns>
    public ISetupBuilder WithEventTypes(Action<IEventTypesBuilder> callback);

    /// <summary>
    /// Sets the aggregate roots through the <see cref="IAggregateRootsBuilder" />.
    /// </summary>
    /// <param name="callback">The builder callback.</param>
    /// <returns>The client builder for continuation.</returns>
    public ISetupBuilder WithAggregateRoots(Action<IAggregateRootsBuilder> callback);

    /// <summary>
    /// Sets the filters through the <see cref="IEventFiltersBuilder" />.
    /// </summary>
    /// <param name="callback">The builder callback.</param>
    /// <returns>The client builder for continuation.</returns>
    public ISetupBuilder WithFilters(Action<IEventFiltersBuilder> callback);

    /// <summary>
    /// Sets the event handlers through the <see cref="IEventHandlersBuilder" />.
    /// </summary>
    /// <param name="callback">The builder callback.</param>
    /// <returns>The client builder for continuation.</returns>
    public ISetupBuilder WithEventHandlers(Action<IEventHandlersBuilder> callback);

    /// <summary>
    /// Sets the event handlers through the <see cref="ProjectionsBuilder" />.
    /// </summary>
    /// <param name="callback">The builder callback.</param>
    /// <returns>The client builder for continuation.</returns>
    public ISetupBuilder WithProjections(Action<IProjectionsBuilder> callback);

    /// <summary>
    /// Sets the event horizons through the <see cref="SubscriptionsBuilder" />.
    /// </summary>
    /// <param name="callback">The builder callback.</param>
    /// <returns>The client builder for continuation.</returns>
    public ISetupBuilder WithEventHorizons(Action<SubscriptionsBuilder> callback);

    /// <summary>
    /// Turns off automatic discovery and registration.
    /// </summary>
    /// <returns>The client builder for continuation.</returns>
    public ISetupBuilder WithoutDiscovery();
}
