// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using System.Threading.Tasks;
using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.DependencyInversion;
using Dolittle.SDK.Events.Handling.Builder.Methods;

namespace Dolittle.SDK.Events.Handling.Builder.Convention.Type;

/// <summary>
/// Methods for building <see cref="IEventHandler"/> instances by convention from an instantiated event handler class.
/// </summary>
public class ConventionTypeEventHandlerBuilder : ConventionEventHandlerBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConventionTypeEventHandlerBuilder"/> class.
    /// </summary>
    /// <param name="eventHandlerType">The <see cref="Type" /> of the event handler.</param>
    public ConventionTypeEventHandlerBuilder(System.Type eventHandlerType)
        : base(eventHandlerType)
    {
    }

    /// <inheritdoc/>
    public override bool TryBuild(
        IEventTypes eventTypes,
        IClientBuildResults buildResults,
        Func<ITenantScopedProviders> tenantScopedProvidersFactory,
        out IEventHandler eventHandler)
        => TryBuild(
            eventTypes,
            method => CreateUntypedHandleMethod(tenantScopedProvidersFactory, method),
            (eventParameterType, method) => CreateTypedHandleMethod(tenantScopedProvidersFactory, eventParameterType, method),
            buildResults,
            out eventHandler);

    IEventHandlerMethod CreateUntypedHandleMethod(Func<ITenantScopedProviders> tenantScopedProvidersFactory, MethodInfo method)
    {
        var eventHandlerSignatureType = method.ReturnType == typeof(Task) ?
            typeof(TaskEventHandlerMethodSignature<>)
            : typeof(VoidEventHandlerMethodSignature<>);
        var eventHandlerSignature = method.CreateDelegate(eventHandlerSignatureType.MakeGenericType(EventHandlerType), null);

        return Activator.CreateInstance(
            typeof(ClassEventHandlerMethod<>).MakeGenericType(EventHandlerType),
            tenantScopedProvidersFactory,
            eventHandlerSignature) as IEventHandlerMethod;
    }

    IEventHandlerMethod CreateTypedHandleMethod(Func<ITenantScopedProviders> tenantScopedProvidersFactory, System.Type eventParameterType, MethodInfo method)
    {
        var eventHandlerSignatureGenericTypeDefinition = method.ReturnType == typeof(Task) ?
            typeof(TaskEventHandlerMethodSignature<,>)
            : typeof(VoidEventHandlerMethodSignature<,>);
        var eventHandlerSignatureType = eventHandlerSignatureGenericTypeDefinition.MakeGenericType(EventHandlerType, eventParameterType);
        var eventHandlerSignature = method.CreateDelegate(eventHandlerSignatureType, null);

        return Activator.CreateInstance(
            typeof(TypedClassEventHandlerMethod<,>).MakeGenericType(EventHandlerType, eventParameterType),
            tenantScopedProvidersFactory,
            eventHandlerSignature) as IEventHandlerMethod;
    }
}
