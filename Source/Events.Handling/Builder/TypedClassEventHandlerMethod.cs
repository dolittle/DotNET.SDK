// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.SDK.Async;
using Dolittle.SDK.DependencyInversion;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.SDK.Events.Handling.Builder
{
    /// <summary>
    /// An implementation of <see cref="IEventHandlerMethod" /> that invokes a method on an event handler instance for an event of a specific type.
    /// </summary>
    /// <typeparam name="TEventHandler">The <see cref="Type" /> of the event handler.</typeparam>
    /// <typeparam name="TEvent">The <see cref="Type" /> of the event.</typeparam>
    public class TypedClassEventHandlerMethod<TEventHandler, TEvent> : IEventHandlerMethod
        where TEventHandler : class
        where TEvent : class
    {
        readonly ITenantScopedProviders _tenantScopedProviders;
        readonly TaskEventHandlerMethodSignature<TEventHandler, TEvent> _method;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypedClassEventHandlerMethod{TEventHandler, TEvent}"/> class.
        /// </summary>
        /// <param name="tenantScopedProviders">The <see cref="ITenantScopedProviders"/> to use for creating instances of the event handler.</param>
        /// <param name="method">The <see cref="TaskEventHandlerMethodSignature{TEvent}"/> method to invoke.</param>
        public TypedClassEventHandlerMethod(ITenantScopedProviders tenantScopedProviders, TaskEventHandlerMethodSignature<TEventHandler, TEvent> method)
        {
            _tenantScopedProviders = tenantScopedProviders;
            _method = method;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypedClassEventHandlerMethod{TEventHandler, TEvent}"/> class.
        /// </summary>
        /// <param name="tenantScopedProviders">The <see cref="ITenantScopedProviders"/> to use for creating instances of the event handler.</param>
        /// <param name="method">The <see cref="VoidEventHandlerMethodSignature{TEvent}" /> method to invoke.</param>
        public TypedClassEventHandlerMethod(ITenantScopedProviders tenantScopedProviders, VoidEventHandlerMethodSignature<TEventHandler, TEvent> method)
            : this(
                tenantScopedProviders,
                (TEventHandler instance, TEvent @event, EventContext context) =>
                {
                    method(instance, @event, context);
                    return Task.CompletedTask;
                })
        {
        }

        /// <inheritdoc/>
        public Task<Try> TryHandle(object @event, EventContext context)
        {
            var eventHandlerInstance = ActivatorUtilities.GetServiceOrCreateInstance<TEventHandler>(_tenantScopedProviders.ForTenant(context.CurrentExecutionContext.Tenant));
            if (eventHandlerInstance == null) throw new CouldNotInstantiateEventHandler(typeof(TEventHandler));
            if (@event is TEvent typedEvent) return _method(eventHandlerInstance, typedEvent, context).TryTask();

            return Task.FromResult<Try>(new TypedEventHandlerMethodInvokedOnEventOfWrongType(typeof(TEvent), @event.GetType()));
        }
    }
}
