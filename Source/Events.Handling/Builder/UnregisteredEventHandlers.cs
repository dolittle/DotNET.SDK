// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Dolittle.SDK.Common;
using Dolittle.SDK.Common.Model;
using Dolittle.SDK.DependencyInversion;
using Dolittle.SDK.Events.Handling.Builder.Convention.Instance;
using Dolittle.SDK.Events.Handling.Builder.Convention.Type;
using Dolittle.SDK.Events.Handling.Internal;
using Dolittle.SDK.Events.Processing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Events.Handling.Builder;

/// <summary>
/// Represents an implementation of <see cref="IUnregisteredEventHandlers"/>.
/// </summary>
public class UnregisteredEventHandlers : UniqueBindings<EventHandlerModelId, IEventHandler>, IUnregisteredEventHandlers
{
    readonly IEnumerable<ProcessorBuilderBinding<ConventionInstanceEventHandlerBuilder>> _instanceBuilders;
    readonly IEnumerable<ProcessorBuilderBinding<ConventionTypeEventHandlerBuilder>> _typedBuilders;

    /// <summary>
    /// Initializes an instance of the <see cref="UnregisteredEventHandlers"/> class.
    /// </summary>
    /// <param name="eventHandlers">The unique <see cref="IEventHandler"/> event handlers.</param>
    /// <param name="instanceBuilders"></param>
    /// <param name="typedBuilders"></param>
    public UnregisteredEventHandlers(
        IUniqueBindings<EventHandlerModelId, IEventHandler> eventHandlers,
        IEnumerable<ProcessorBuilderBinding<ConventionInstanceEventHandlerBuilder>> instanceBuilders,
        IEnumerable<ProcessorBuilderBinding<ConventionTypeEventHandlerBuilder>> typedBuilders)
        : base(eventHandlers)
    {
        _instanceBuilders = instanceBuilders;
        _typedBuilders = typedBuilders;
    }

    /// <inheritdoc />
    public void Register(
        IEventProcessors eventProcessors,
        IEventProcessingConverter processingConverter,
        TenantScopedProvidersBuilder tenantScopedProvidersBuilder,
        ILoggerFactory loggerFactory,
        CancellationToken cancellationToken)
    {
        foreach (var eventHandler in Values)
        {
            eventProcessors.Register(
                new EventHandlerProcessor(
                    eventHandler,
                    processingConverter,
                    loggerFactory.CreateLogger<EventHandlerProcessor>()),
                new EventHandlerProtocol(),
                cancellationToken);
            AddToContainer(eventHandler.Identifier, tenantScopedProvidersBuilder);
        }
    }

    void AddToContainer(EventHandlerId eventHandlerId, TenantScopedProvidersBuilder tenantScopedProvidersBuilder)
    {
        var typedBuilderBinding = _typedBuilders.FirstOrDefault(_ => _.Identifier.Id.Equals(eventHandlerId.Value));
        if (typedBuilderBinding != null)
        {
            tenantScopedProvidersBuilder.AddTenantServices((_, collection) => collection.AddScoped(typedBuilderBinding.ProcessorBuilder.EventHandlerType));
            return;
        }
        
        var instanceBuilderBinding = _instanceBuilders.FirstOrDefault(_ => _.Identifier.Id.Equals(eventHandlerId.Value));
        if (instanceBuilderBinding != null)
        {
            tenantScopedProvidersBuilder.AddTenantServices((_, collection) => collection.AddSingleton(instanceBuilderBinding.ProcessorBuilder.EventHandlerType, instanceBuilderBinding.ProcessorBuilder.EventHandlerInstance));
        }
    }
}
