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
        public override void BuildAndRegister(
            IEventProcessors eventProcessors,
            IEventTypes eventTypes,
            IEventProcessingConverter processingConverter,
            ITenantScopedProviders tenantScopedProviders,
            ILoggerFactory loggerFactory,
            CancellationToken cancellation)
            => BuildAndRegister(
                eventProcessors,
                eventTypes,
                processingConverter,
                method => CreateUntypedHandleMethod(tenantScopedProviders, method),
                (eventParameterType, method) => CreateTypedHandleMethod(tenantScopedProviders, eventParameterType, method),
                loggerFactory,
                loggerFactory.CreateLogger(GetType()),
                cancellation);

        IEventHandlerMethod CreateUntypedHandleMethod(ITenantScopedProviders tenantScopedProviders, MethodInfo method)
        {
            var eventHandlerSignatureType = method.ReturnType == typeof(Task) ?
                                    typeof(TaskEventHandlerMethodSignature<>)
                                    : typeof(VoidEventHandlerMethodSignature<>);
            var eventHandlerSignature = method.CreateDelegate(eventHandlerSignatureType.MakeGenericType(EventHandlerType), null);

            return Activator.CreateInstance(
                typeof(ClassEventHandlerMethod<>).MakeGenericType(EventHandlerType),
                tenantScopedProviders,
                eventHandlerSignature) as IEventHandlerMethod;
        }

        IEventHandlerMethod CreateTypedHandleMethod(ITenantScopedProviders tenantScopedProviders, Type eventParameterType, MethodInfo method)
        {
            var eventHandlerSignatureGenericTypeDefinition = method.ReturnType == typeof(Task) ?
                                                typeof(TaskEventHandlerMethodSignature<,>)
                                                : typeof(VoidEventHandlerMethodSignature<,>);
            var eventHandlerSignatureType = eventHandlerSignatureGenericTypeDefinition.MakeGenericType(EventHandlerType, eventParameterType);
            var eventHandlerSignature = method.CreateDelegate(eventHandlerSignatureType, null);

            return Activator.CreateInstance(
                typeof(TypedClassEventHandlerMethod<,>).MakeGenericType(EventHandlerType, eventParameterType),
                tenantScopedProviders,
                eventHandlerSignature) as IEventHandlerMethod;
        }
    }
}