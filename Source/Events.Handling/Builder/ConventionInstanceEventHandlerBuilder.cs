// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.DependencyInversion;
using Dolittle.SDK.Events.Processing;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Events.Handling.Builder
{
    /// <summary>
    /// Methods for building <see cref="IEventHandler"/> instances by convention from an instantiated event handler class.
    /// </summary>
    public class ConventionInstanceEventHandlerBuilder : ConventionEventHandlerBuilder
    {
        readonly object _eventHandlerInstance;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConventionInstanceEventHandlerBuilder"/> class.
        /// </summary>
        /// <param name="eventHandlerInstance">The event handler instance.</param>
        public ConventionInstanceEventHandlerBuilder(object eventHandlerInstance)
            : base(eventHandlerInstance.GetType())
        {
            _eventHandlerInstance = eventHandlerInstance;
        }

        /// <inheritdoc/>
        public override void BuildAndRegister(
            IEventProcessors eventProcessors,
            IEventTypes eventTypes,
            IEventProcessingConverter processingConverter,
            IContainer container,
            ILoggerFactory loggerFactory,
            CancellationToken cancellation)
            => BuildAndRegister(
                eventProcessors,
                eventTypes,
                processingConverter,
                CreateUntypedHandleMethod,
                CreateTypedHandleMethod,
                loggerFactory,
                loggerFactory.CreateLogger(GetType()),
                cancellation);

        IEventHandlerMethod CreateUntypedHandleMethod(MethodInfo method)
        {
            var eventHandlerSignatureType = method.ReturnType == typeof(Task) ?
                                    typeof(TaskEventHandlerMethodSignature<>)
                                    : typeof(VoidEventHandlerMethodSignature<>);
            var eventHandlerSignature = method.CreateDelegate(eventHandlerSignatureType.MakeGenericType(EventHandlerType), null);

            return Activator.CreateInstance(
                typeof(InstanceEventHandlerMethod<>).MakeGenericType(EventHandlerType),
                _eventHandlerInstance,
                eventHandlerSignature) as IEventHandlerMethod;
        }

        IEventHandlerMethod CreateTypedHandleMethod(Type eventParameterType, MethodInfo method)
        {
            var eventHandlerSignatureGenericTypeDefinition = method.ReturnType == typeof(Task) ?
                                                typeof(TaskEventHandlerMethodSignature<,>)
                                                : typeof(VoidEventHandlerMethodSignature<,>);
            var eventHandlerSignatureType = eventHandlerSignatureGenericTypeDefinition.MakeGenericType(EventHandlerType, eventParameterType);
            var eventHandlerSignature = method.CreateDelegate(eventHandlerSignatureType, null);

            return Activator.CreateInstance(
                typeof(TypedInstanceEventHandlerMethod<,>).MakeGenericType(EventHandlerType, eventParameterType),
                _eventHandlerInstance,
                eventHandlerSignature) as IEventHandlerMethod;
        }
    }
}