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
    public class ConventionTypeEventHandlerBuilder : ConventionEventHandlerBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConventionTypeEventHandlerBuilder"/> class.
        /// </summary>
        /// <param name="eventHandlerType">The <see cref="Type" /> of the event handler.</param>
        public ConventionTypeEventHandlerBuilder(Type eventHandlerType)
            : base(eventHandlerType)
        {
        }

        /// <inheritdoc/>
        public override BuildEventHandlerResult BuildAndRegister(
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
                method => CreateUntypedHandleMethod(container, method),
                (eventParameterType, method) => CreateTypedHandleMethod(container, eventParameterType, method),
                loggerFactory,
                loggerFactory.CreateLogger(GetType()),
                cancellation);

        IEventHandlerMethod CreateUntypedHandleMethod(IContainer container, MethodInfo method)
        {
            var eventHandlerSignatureType = method.ReturnType == typeof(Task) ?
                                    typeof(TaskEventHandlerMethodSignature)
                                    : typeof(VoidEventHandlerMethodSignature);
            var eventHandlerSignature = method.CreateDelegate(eventHandlerSignatureType, null);

            return eventHandlerSignature switch
            {
                TaskEventHandlerMethodSignature signature => new ClassEventHandlerMethod(EventHandlerType, container, signature),
                VoidEventHandlerMethodSignature signature => new ClassEventHandlerMethod(EventHandlerType, container, signature),
                _ => null
            };
        }

        IEventHandlerMethod CreateTypedHandleMethod(IContainer container, Type eventParameterType, MethodInfo method)
        {
            var eventHandlerSignatureGenericTypeDefinition = method.ReturnType == typeof(Task) ?
                                                typeof(TaskEventHandlerMethodSignature<>)
                                                : typeof(VoidEventHandlerMethodSignature<>);
            var eventHandlerSignatureType = eventHandlerSignatureGenericTypeDefinition.MakeGenericType(eventParameterType);
            var eventHandlerSignature = method.CreateDelegate(eventHandlerSignatureType, null);

            return Activator.CreateInstance(
                typeof(TypedClassEventHandlerMethod<>).MakeGenericType(eventParameterType),
                EventHandlerType,
                container,
                eventHandlerSignature) as IEventHandlerMethod;
        }
    }
}