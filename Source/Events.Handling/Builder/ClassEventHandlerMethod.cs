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
    public class ClassEventHandlerMethod<TEventHandler> : IEventHandlerMethod
        where TEventHandler : class
    {
        readonly ITenantScopedProviders _tenantScopedProviders;
        readonly TaskEventHandlerMethodSignature<TEventHandler> _method;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassEventHandlerMethod{TEventHandler}"/> class.
        /// </summary>
        /// <param name="tenantScopedProviders">The <see cref="ITenantScopedProviders"/> to use for creating instances of the event handler.</param>
        /// <param name="method">The <see cref="TaskEventHandlerMethodSignature{TEvent}"/> method to invoke.</param>
        public ClassEventHandlerMethod(ITenantScopedProviders tenantScopedProviders, TaskEventHandlerMethodSignature<TEventHandler> method)
        {
            _tenantScopedProviders = tenantScopedProviders;
            _method = method;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassEventHandlerMethod{TEventHandler}"/> class.
        /// </summary>
        /// <param name="tenantScopedProviders">The <see cref="ITenantScopedProviders"/> to use for creating instances of the event handler.</param>
        /// <param name="method">The <see cref="VoidEventHandlerMethodSignature{TEvent}" /> method to invoke.</param>
        public ClassEventHandlerMethod(ITenantScopedProviders tenantScopedProviders, VoidEventHandlerMethodSignature<TEventHandler> method)
            : this(
                tenantScopedProviders,
                (TEventHandler instance, object @event, EventContext context) =>
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
            return _method(eventHandlerInstance, @event, context).TryTask();
        }
    }
}
