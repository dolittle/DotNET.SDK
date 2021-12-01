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
        readonly Func<ITenantScopedProviders> _tenantScopedProvidersFactory;
        readonly TaskEventHandlerMethodSignature<TEventHandler, TEvent> _method;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypedClassEventHandlerMethod{TEventHandler, TEvent}"/> class.
        /// </summary>
        /// <param name="tenantScopedProvidersFactory">The <see cref="ITenantScopedProviders"/> to use for creating instances of the event handler.</param>
        /// <param name="method">The <see cref="TaskEventHandlerMethodSignature{TEvent}"/> method to invoke.</param>
        public TypedClassEventHandlerMethod(Func<ITenantScopedProviders> tenantScopedProvidersFactory, TaskEventHandlerMethodSignature<TEventHandler, TEvent> method)
        {
            _tenantScopedProvidersFactory = tenantScopedProvidersFactory;
            _method = method;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypedClassEventHandlerMethod{TEventHandler, TEvent}"/> class.
        /// </summary>
        /// <param name="tenantScopedProvidersFactory">The <see cref="ITenantScopedProviders"/> to use for creating instances of the event handler.</param>
        /// <param name="method">The <see cref="VoidEventHandlerMethodSignature{TEvent}" /> method to invoke.</param>
        public TypedClassEventHandlerMethod(Func<ITenantScopedProviders> tenantScopedProvidersFactory, VoidEventHandlerMethodSignature<TEventHandler, TEvent> method)
            : this(
                tenantScopedProvidersFactory,
                (TEventHandler instance, TEvent @event, EventContext context) =>
                {
                    method(instance, @event, context);
                    return Task.CompletedTask;
                })
        { }

        /// <inheritdoc/>
        public async Task<Try> TryHandle(object @event, EventContext context)
        {
            if (!(@event is TEvent typedEvent))
            {
                return new TypedEventHandlerMethodInvokedOnEventOfWrongType(typeof(TEvent), @event.GetType());
            }

            using var scope = _tenantScopedProvidersFactory().ForTenant(context.CurrentExecutionContext.Tenant).CreateScope();
            var eventHandler = scope.ServiceProvider.GetService<TEventHandler>();
            if (eventHandler == null)
            {
                throw new CouldNotInstantiateEventHandler(typeof(TEventHandler));
            }

            return await _method(eventHandler, typedEvent, context).TryTask().ConfigureAwait(false);
        }
    }
}
