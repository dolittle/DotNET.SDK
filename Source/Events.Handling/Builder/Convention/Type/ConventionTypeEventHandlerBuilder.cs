// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using System.Threading.Tasks;
using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Events.Handling.Builder.Methods;

namespace Dolittle.SDK.Events.Handling.Builder.Convention.Type;

/// <summary>
/// Methods for building <see cref="IEventHandler"/> instances by convention from an instantiated event handler class.
/// </summary>
public class ConventionTypeEventHandlerBuilder : ConventionEventHandlerBuilder, IEquatable<ConventionTypeEventHandlerBuilder>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConventionTypeEventHandlerBuilder"/> class.
    /// </summary>
    /// <param name="eventHandlerType">The <see cref="Type" /> of the event handler.</param>
    /// <param name="decorator">The <see cref="EventHandlerAttribute"/>.</param>
    public ConventionTypeEventHandlerBuilder(System.Type eventHandlerType, EventHandlerAttribute decorator)
        : base(decorator, eventHandlerType)
    {
    }
    
    /// <inheritdoc />
    public bool Equals(ConventionTypeEventHandlerBuilder other)
        => base.Equals(other);

    /// <inheritdoc />
    public override bool Equals(object other)
        => Equals(other as ConventionTypeEventHandlerBuilder);

    /// <inheritdoc />
    public override int GetHashCode()
        => base.GetHashCode();

    /// <inheritdoc/>
    public override bool TryBuild(
        IEventTypes eventTypes,
        IClientBuildResults buildResults,
        out IEventHandler eventHandler)
        => TryBuild(
            eventTypes,
            CreateUntypedHandleMethod,
            CreateTypedHandleMethod,
            buildResults,
            out eventHandler);

    IEventHandlerMethod CreateUntypedHandleMethod(MethodInfo method)
    {
        var eventHandlerSignatureType = method.ReturnType == typeof(Task) ?
            typeof(TaskEventHandlerMethodSignature<>)
            : typeof(VoidEventHandlerMethodSignature<>);
        var eventHandlerSignature = method.CreateDelegate(eventHandlerSignatureType.MakeGenericType(EventHandlerType), null);

        return Activator.CreateInstance(
            typeof(ClassEventHandlerMethod<>).MakeGenericType(EventHandlerType),
            eventHandlerSignature) as IEventHandlerMethod;
    }

    IEventHandlerMethod CreateTypedHandleMethod(System.Type eventParameterType, MethodInfo method)
    {
        var eventHandlerSignatureGenericTypeDefinition = method.ReturnType == typeof(Task) ?
            typeof(TaskEventHandlerMethodSignature<,>)
            : typeof(VoidEventHandlerMethodSignature<,>);
        var eventHandlerSignatureType = eventHandlerSignatureGenericTypeDefinition.MakeGenericType(EventHandlerType, eventParameterType);
        var eventHandlerSignature = method.CreateDelegate(eventHandlerSignatureType, null);

        return Activator.CreateInstance(
            typeof(TypedClassEventHandlerMethod<,>).MakeGenericType(EventHandlerType, eventParameterType),
            eventHandlerSignature) as IEventHandlerMethod;
    }
}
