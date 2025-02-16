// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Events.Handling.Builder.Methods;

namespace Dolittle.SDK.Events.Handling.Builder.Convention;

/// <summary>
/// Methods for building <see cref="IEventHandler"/> instances by convention from an instantiated event handler class.
/// </summary>
public abstract class ConventionEventHandlerBuilder : ICanTryBuildEventHandler, IEquatable<ICanTryBuildEventHandler>
{
    const string MethodName = "Handle";
    
    readonly EventHandlerModelId _identifier;
    readonly object _eventHandlerInstance;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConventionEventHandlerBuilder"/> class.
    /// </summary>
    /// <param name="identifier">The <see cref="EventHandlerAttribute"/> decorator.</param>
    /// <param name="eventHandlerType">The event handler <see cref="Type" />.</param>
    /// <param name="eventHandlerInstance">The optional instance of the event handler class.</param>
    protected ConventionEventHandlerBuilder(EventHandlerModelId identifier, System.Type eventHandlerType, object eventHandlerInstance = default)
    {
        EventHandlerType = eventHandlerType;
        _identifier = identifier;
        _eventHandlerInstance = eventHandlerInstance;
    }

    /// <summary>
    /// Gets the <see cref="Type" /> of the event handler.
    /// </summary>
    public System.Type EventHandlerType { get; }

    /// <inheritdoc />
    public abstract bool TryBuild(
        EventHandlerModelId identifier,
        IEventTypes eventTypes,
        IClientBuildResults buildResults,
        out IEventHandler eventHandler);

    /// <inheritdoc />
    public bool Equals(ICanTryBuildEventHandler other)
    {
        if (other == null)
        {
            return false;
        }
        
        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return other is ConventionEventHandlerBuilder otherBuilder
            && EventHandlerType == otherBuilder.EventHandlerType
            && _eventHandlerInstance.Equals(otherBuilder._eventHandlerInstance);
    }

    /// <inheritdoc />
    public override int GetHashCode()
        => HashCode.Combine(_identifier, _eventHandlerInstance, EventHandlerType);
    
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
        buildResults.AddInformation(_identifier, $"Building from type {EventHandlerType}");

        var eventTypesToMethods = new Dictionary<EventType, IEventHandlerMethod>();

        if (!TryBuildHandlerMethods(
                _identifier.Id,
                eventTypes,
                createUntypedHandlerMethod,
                createTypedHandlerMethod,
                eventTypesToMethods,
                buildResults))
        {
            return false;
        }

        eventHandler = new EventHandler(_identifier, eventTypesToMethods);

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
        var allDeclaredMethods = EventHandlerType.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic);
        
        var hasWrongMethods = !TryAddDecoratedHandlerMethods(
                allDeclaredMethods,
                eventHandlerId,
                createUntypedHandlerMethod,
                eventTypesToMethods,
                buildResults)
            | !TryAddConventionHandlerMethods(
                allDeclaredMethods,
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
        buildResults.AddFailure(_identifier, $"There are no event handler methods to register in event handler {EventHandlerType}. An event handler method either needs to be decorated with [{nameof(HandlesAttribute)}] or have the name {MethodName}");
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
                buildResults.AddFailure(_identifier, $"{method} has no parameters, but is decorated with [{nameof(HandlesAttribute)}]. An event handler method should take in as parameters an event and an {nameof(EventContext)}");
                shouldAddHandler = false;
            }

            if (!ParametersAreOkay(method, buildResults))
            {
                shouldAddHandler = false;
            }

            if (eventParameterType != typeof(object))
            {
                buildResults.AddFailure(_identifier, $"{method} should only handle an event of type object");
                shouldAddHandler = false;
            }

            if (!method.IsPublic)
            {
                buildResults.AddFailure(_identifier, $"{method} has the signature of an event handler method, but is not public. Event handler methods needs to be public");
                shouldAddHandler = false;
            }
            
            switch (shouldAddHandler)
            {
                case true when !eventTypesToMethods.TryAdd(eventType, createUntypedHandlerMethod(method)):
                    allMethodsAdded = false;
                    buildResults.AddFailure(_identifier, $"Multiple handlers for {eventType}");
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
                buildResults.AddFailure(_identifier, $"{method} has no parameters. An event handler method should take in as parameters an event and an {nameof(EventContext)}");
                shouldAddHandler = false;
            }

            if (eventParameterType == typeof(object))
            {
                buildResults.AddFailure(_identifier, $"{method} cannot handle an untyped event when not decorated with [{nameof(HandlesAttribute)}]");
                shouldAddHandler = false;
            }

            if (!eventTypes.HasFor(eventParameterType))
            {
                buildResults.AddFailure(_identifier, $"{method} handles event of type {eventParameterType}, but it is not associated to any event type");
                shouldAddHandler = false;
            }

            if (!ParametersAreOkay(method, buildResults))
            {
                shouldAddHandler = false;
            }

            if (!method.IsPublic)
            {
                buildResults.AddFailure(_identifier, $"{method} has the signature of an event handler method, but is not public. Event handler methods needs to be public");
                shouldAddHandler = false;
            }

            switch (shouldAddHandler)
            {
                case true when !eventTypesToMethods.TryAdd(eventTypes.GetFor(eventParameterType), createTypedHandlerMethod(eventParameterType, method)):
                    allMethodsAdded = false;
                    buildResults.AddFailure(_identifier, $"Multiple handlers for {eventParameterType}");
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
        var hasErrors = false;
        if (!SecondMethodParameterIsEventContext(method))
        {
            buildResults.AddFailure(_identifier, $"{method} needs to have two parameters where the second parameter is {nameof(EventContext)}");
            hasErrors = true;
        }

        if (!MethodHasNoExtraParameters(method))
        {
            buildResults.AddFailure(_identifier, $"{method} needs to only have two parameters where the first is the event to handle and the second is {nameof(EventContext)}");
            hasErrors = true;
        }

        if (MethodReturnsAsyncVoid(method) || (!MethodReturnsVoid(method) && !MethodReturnsTask(method)))
        {
            buildResults.AddFailure(_identifier, $"{method} needs to return either {typeof(void)} or {typeof(Task)}");
            hasErrors = true;
        }

        return !hasErrors;
    }
    
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
