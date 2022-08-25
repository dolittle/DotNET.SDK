// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using Dolittle.SDK.Common;
using Dolittle.SDK.ApplicationModel;
using Dolittle.SDK.DependencyInversion;
using Dolittle.SDK.Events.Handling.Builder.Convention.Instance;
using Dolittle.SDK.Events.Handling.Builder.Convention.Type;
using Dolittle.SDK.Events.Handling.Internal;
using Dolittle.SDK.Events.Processing;
using Dolittle.SDK.Tenancy;
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
        AddTenantScopedServices = AddToContainer;
    }

    /// <inheritdoc />
    public ConfigureTenantServices AddTenantScopedServices { get; }

    /// <inheritdoc />
    public void Register(
        IEventProcessors eventProcessors,
        IEventProcessingConverter processingConverter,
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
        }
    }

    void AddToContainer(TenantId tenant, IServiceCollection serviceCollection)
    {
        foreach (var (_, builder) in _typedBuilders)
        {
            serviceCollection.AddScoped(builder.EventHandlerType);
        }

        foreach (var (_, builder) in _instanceBuilders)
        {
            serviceCollection.AddSingleton(builder.EventHandlerType, builder.EventHandlerInstance);
        }
    }
}
