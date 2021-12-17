// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Dolittle.SDK.DependencyInversion;
using Dolittle.SDK.Events.Handling.Internal;
using Dolittle.SDK.Events.Processing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Events.Handling.Builder;

/// <summary>
/// Represents an implementation of <see cref="IUnregisteredEventHandlers"/>.
/// </summary>
public class UnregisteredEventHandlers : IUnregisteredEventHandlers
{
    readonly IEventHandlerBindings _allBindings;
    
    /// <summary>
    /// Initializes an instance of the <see cref="UnregisteredEventHandlers"/> class.
    /// </summary>
    /// <param name="allBindings"></param>
    public UnregisteredEventHandlers(IEventHandlerBindings allBindings)
    {
        _allBindings = allBindings;
    }

    /// <inheritdoc />
    public void Register(
        IEventProcessors eventProcessors,
        IEventProcessingConverter processingConverter,
        TenantScopedProvidersBuilder tenantScopedProvidersBuilder,
        ILoggerFactory loggerFactory,
        CancellationToken cancelConnectToken,
        CancellationToken stopProcessingToken)
    {
        foreach (var eventHandler in _allBindings.Values)
        {
            eventProcessors.Register(
                new EventHandlerProcessor(
                    eventHandler,
                    processingConverter,
                    loggerFactory.CreateLogger<EventHandlerProcessor>()),
                new EventHandlerProtocol(),
                cancelConnectToken,
                stopProcessingToken);
            AddToContainer(eventHandler.Identifier, tenantScopedProvidersBuilder);
        }
    }

    void AddToContainer(EventHandlerId eventHandlerId, TenantScopedProvidersBuilder tenantScopedProvidersBuilder)
    {
        if (_allBindings.Instances.HasFor(eventHandlerId))
        {
            tenantScopedProvidersBuilder.AddTenantServices((_, collection) => collection.AddScoped(_allBindings.Typed.GetFor(eventHandlerId)));
        }
        else if (_allBindings.Typed.HasFor(eventHandlerId))
        {
            tenantScopedProvidersBuilder.AddTenantServices((_, collection) => collection.AddSingleton(_allBindings.Instances.GetFor(eventHandlerId)));
        }
    }
}
