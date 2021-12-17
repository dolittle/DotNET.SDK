// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.DependencyInversion;
using Dolittle.SDK.Events.Handling.Builder.Methods;

namespace Dolittle.SDK.Events.Handling.Builder.Convention;

/// <summary>
/// Methods for building <see cref="IEventHandler"/> instances by convention from an instantiated event handler class.
/// </summary>
public abstract class ConventionEventHandlerBuilder
{
    const string MethodName = "Handle";

    /// <summary>
    /// Initializes a new instance of the <see cref="ConventionEventHandlerBuilder"/> class.
    /// </summary>
    /// <param name="eventHandlerType">The event handler <see cref="Type" />.</param>
    protected ConventionEventHandlerBuilder(System.Type eventHandlerType) => EventHandlerType = eventHandlerType;

    /// <summary>
    /// Gets the <see cref="Type" /> of the event handler.
    /// </summary>
    protected System.Type EventHandlerType { get; }

    /// <summary>
    /// Builds event handler.
    /// </summary>
    /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
    /// <param name="buildResults">The <see cref="IClientBuildResults"/>.</param>
    /// <param name="tenantScopedProvidersFactory">The <see cref="System.Func{TResult}"/> for getting the <see cref="ITenantScopedProviders"/>.</param>
    /// <param name="eventHandler">The built <see cref="IEventHandler"/>.</param>
    public abstract bool TryBuild(
        IEventTypes eventTypes,
        IClientBuildResults buildResults,
        System.Func<ITenantScopedProviders> tenantScopedProvidersFactory,
        out IEventHandler eventHandler);
    
    /// <summary>
    /// Builds event handler.
    /// </summary>
    /// <param name="eventTypes">The <see cref="IEventTypes" />.</param>
    /// <param name="createUntypedHandlerMethod">The <see cref="CreateUntypedHandleMethod" /> callback.</param>
    /// <param name="createTypedHandlerMethod">The <see cref="CreateTypedHandleMethod" /> callback.</param>
    /// <param name="buildResults">The <see cref="IClientBuildResults"/>.</param>
    /// <param name="eventHandler">The built <see cref="IEventHandler"/>.</param>
    protected bool TryBuild(
        IEventTypes eventTypes,
        CreateUntypedHandleMethod createUntypedHandlerMethod,
        CreateTypedHandleMethod createTypedHandlerMethod,
        IClientBuildResults buildResults,
        out IEventHandler eventHandler)
    {
        eventHandler = default;
        buildResults.AddInformation($"Building event handler from type {EventHandlerType}");
        var attribute = GetAttribute();
        
        buildResults.AddInformation($"Building {(attribute.Partitioned ? "partitioned" : "unpartitioned")} event handler {attribute.Identifier} processing events in scope {attribute.Scope} from type {EventHandlerType}");

        var eventTypesToMethods = new Dictionary<EventType, IEventHandlerMethod>();

        if (!TryBuildHandlerMethods(
                attribute.Identifier,
                eventTypes,
                createUntypedHandlerMethod,
                createTypedHandlerMethod,
                eventTypesToMethods,
                buildResults))
        {
            return false;
        }

        eventHandler = attribute.HasAlias
            ? new EventHandler(attribute.Identifier, attribute.Alias, attribute.Scope, attribute.Partitioned, eventTypesToMethods)
            : new EventHandler(attribute.Identifier, EventHandlerType.Name, attribute.Scope, attribute.Partitioned, eventTypesToMethods);

        return true;
    }

    bool TryBuildHandlerMethods(
        EventHandlerId eventHandlerId,
        IEventTypes eventTypes,
        CreateUntypedHandleMethod createUntypedHandlerMethod,
        CreateTypedHandleMethod createTypedHandlerMethod,
        IDictionary<EventType, IEventHandlerMethod> eventTypesToMethods,
        IClientBuildResults buildResults)
    {
        var publicMethods = EventHandlerType.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic);
        var hasWrongMethods = !TryAddDecoratedHandlerMethods(
                publicMethods,
                eventHandlerId,
                createUntypedHandlerMethod,
                eventTypesToMethods,
                buildResults)
            || !TryAddConventionHandlerMethods(
                publicMethods,
                eventHandlerId,
                eventTypes,
                createTypedHandlerMethod,
                eventTypesToMethods,
                buildResults);

        if (hasWrongMethods)
        {
            return false;
        }

        if (eventTypesToMethods.Count != 0)
        {
            return true;
        }
        buildResults.AddFailure($"There are no event handler methods to register in event handler {EventHandlerType}. An event handler method either needs to be decorated with [{nameof(HandlesAttribute)}] or have the name {MethodName}");
        return false;
    }

    bool TryAddDecoratedHandlerMethods(
        IEnumerable<MethodInfo> methods,
        EventHandlerId eventHandlerId,
        CreateUntypedHandleMethod createUntypedHandlerMethod,
        IDictionary<EventType, IEventHandlerMethod> eventTypesToMethods,
        IClientBuildResults buildResults)
    {
        var allMethodsAdded = true;
        foreach (var method in methods.Where(IsDecoratedHandlerMethod))
        {
            var shouldAddHandler = true;
            var eventType = (method.GetCustomAttributes(typeof(HandlesAttribute), true)[0] as HandlesAttribute)?.EventType;
            if (!TryGetFirstMethodParameterType(method, out var eventParameterType))
            {
                buildResults.AddFailure($"Event handler method {method} on event handler {EventHandlerType} has no parameters, but is decorated with [{nameof(HandlesAttribute)}]. An event handler method should take in as parameters an event and an {nameof(EventContext)}");
                shouldAddHandler = false;
            }

            if (!ParametersAreOkay(method, buildResults))
            {
                shouldAddHandler = false;
            }

            if (eventParameterType != typeof(object))
            {
                buildResults.AddFailure($"Event handler method {method} on event handler {EventHandlerType} should only handle an event of type object");
                shouldAddHandler = false;
            }

            if (!method.IsPublic)
            {
                buildResults.AddFailure($"Method {method} on event handler {EventHandlerType} has the signature of an event handler method, but is not public. Event handler methods needs to be public");
                shouldAddHandler = false;
            }

            switch (shouldAddHandler)
            {
                case true when !eventTypesToMethods.TryAdd(eventType, createUntypedHandlerMethod(method)):
                    allMethodsAdded = false;
                    buildResults.AddFailure($"Event type {eventType} is already handled in event handler {eventHandlerId}");
                    break;
                case false:
                    allMethodsAdded = false;
                    break;
            }
        }
        return allMethodsAdded;
    }

    bool TryAddConventionHandlerMethods(
        IEnumerable<MethodInfo> methods,
        EventHandlerId eventHandlerId,
        IEventTypes eventTypes,
        CreateTypedHandleMethod createTypedHandlerMethod,
        IDictionary<EventType, IEventHandlerMethod> eventTypesToMethods,
        IClientBuildResults buildResults)
    {
        var allMethodsAdded = true;
        foreach (var method in methods.Where(_ => !IsDecoratedHandlerMethod(_) && _.Name == MethodName))
        {
            var shouldAddHandler = true;
            if (!TryGetFirstMethodParameterType(method, out var eventParameterType))
            {
                buildResults.AddFailure($"Event handler method {method} on event handler {EventHandlerType} has no parameters. An event handler method should take in as parameters an event and an {nameof(EventContext)}");
                shouldAddHandler = false;
            }

            if (eventParameterType == typeof(object))
            {
                buildResults.AddFailure($"Event handler method {method} on event handler {EventHandlerType} cannot handle an untyped event when not decorated with [{nameof(HandlesAttribute)}]");
                shouldAddHandler = false;
            }

            if (!eventTypes.HasFor(eventParameterType))
            {
                buildResults.AddFailure($"Event handler method {method} on event handler {EventHandlerType} handles event of type {eventParameterType}, but it is not associated to any event type");
                shouldAddHandler = false;
            }

            if (!ParametersAreOkay(method, buildResults))
            {
                shouldAddHandler = false;
            }

            if (!method.IsPublic)
            {
                buildResults.AddFailure($"Method {method} on event handler {EventHandlerType} has the signature of an event handler method, but is not public. Event handler methods needs to be public");
                shouldAddHandler = false;
            }

            switch (shouldAddHandler)
            {
                case true when !eventTypesToMethods.TryAdd(eventTypes.GetFor(eventParameterType), createTypedHandlerMethod(eventParameterType, method)):
                    allMethodsAdded = false;
                    buildResults.AddFailure($"Event type {eventParameterType} is already handled in event handler {eventHandlerId}");
                    break;
                case false:
                    allMethodsAdded = false;
                    break;
            }
        }
        return allMethodsAdded;
    }

    bool ParametersAreOkay(MethodInfo method, IClientBuildResults buildResults)
    {
        if (!SecondMethodParameterIsEventContext(method))
        {
            buildResults.AddFailure($"Event handler method {method} on event handler {EventHandlerType} needs to have two parameters where the second parameter is {nameof(EventContext)}");
        }

        if (!MethodHasNoExtraParameters(method))
        {
            buildResults.AddFailure($"Event handler method {method} on event handler {EventHandlerType} needs to only have two parameters where the first is the event to handle and the second is {nameof(EventContext)}");
        }

        if (!MethodReturnsAsyncVoid(method) && (MethodReturnsVoid(method) || MethodReturnsTask(method)))
        {
            return true;
        }
        buildResults.AddFailure($"Event handler method {method} on event handler {EventHandlerType} needs to return either {typeof(void)} or {typeof(Task)}");
        return false;
    }

    EventHandlerAttribute GetAttribute()
        => EventHandlerType.GetCustomAttributes(typeof(EventHandlerAttribute), true).First() as EventHandlerAttribute;

    static bool TryGetFirstMethodParameterType(MethodInfo method, out System.Type type)
    {
        type = default;
        if (method.GetParameters().Length == 0)
        {
            return false;
        }

        type = method.GetParameters()[0].ParameterType;
        return true;
    }

    static bool IsDecoratedHandlerMethod(MethodInfo method)
        => method.GetCustomAttributes(typeof(HandlesAttribute), true).FirstOrDefault() != default;

    static bool SecondMethodParameterIsEventContext(MethodInfo method)
        => method.GetParameters().Length > 1 && method.GetParameters()[1].ParameterType == typeof(EventContext);

    static bool MethodHasNoExtraParameters(MethodInfo method)
        => method.GetParameters().Length == 2;

    static bool MethodReturnsTask(MethodInfo method)
        => method.ReturnType == typeof(Task);

    static bool MethodReturnsVoid(MethodInfo method)
        => method.ReturnType == typeof(void);

    static bool MethodReturnsAsyncVoid(MethodInfo method)
    {
        var asyncAttribute = typeof(AsyncStateMachineAttribute);
        var isAsyncMethod = (AsyncStateMachineAttribute)method.GetCustomAttribute(asyncAttribute) != null;
        return isAsyncMethod && MethodReturnsVoid(method);
    }
}
