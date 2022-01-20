// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.SDK.Async;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.SDK.Events.Handling.Builder.Methods;

/// <summary>
/// An implementation of <see cref="IEventHandlerMethod" /> that invokes a method on an event handler instance for an event of a specific type.
/// </summary>
/// <typeparam name="TEventHandler">The <see cref="Type" /> of the event handler.</typeparam>
/// <typeparam name="TEvent">The <see cref="Type" /> of the event.</typeparam>
public class TypedClassEventHandlerMethod<TEventHandler, TEvent> : IEventHandlerMethod
    where TEventHandler : class
    where TEvent : class
{
    readonly TaskEventHandlerMethodSignature<TEventHandler, TEvent> _method;

    /// <summary>
    /// Initializes a new instance of the <see cref="TypedClassEventHandlerMethod{TEventHandler, TEvent}"/> class.
    /// </summary>
    /// <param name="method">The <see cref="TaskEventHandlerMethodSignature{TEvent}"/> method to invoke.</param>
    public TypedClassEventHandlerMethod(TaskEventHandlerMethodSignature<TEventHandler, TEvent> method)
    {
        _method = method;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TypedClassEventHandlerMethod{TEventHandler, TEvent}"/> class.
    /// </summary>
    /// <param name="method">The <see cref="VoidEventHandlerMethodSignature{TEvent}" /> method to invoke.</param>
    public TypedClassEventHandlerMethod(VoidEventHandlerMethodSignature<TEventHandler, TEvent> method)
        : this(
            (instance, @event, context) =>
            {
                method(instance, @event, context);
                return Task.CompletedTask;
            })
    { }

    /// <inheritdoc/>
    public async Task<Try> TryHandle(object @event, EventContext context, IServiceProvider serviceProvider)
    {
        if (@event is not TEvent typedEvent)
        {
            return new TypedEventHandlerMethodInvokedOnEventOfWrongType(typeof(TEvent), @event.GetType());
        }

        using var scope = serviceProvider.CreateScope();
        var eventHandler = scope.ServiceProvider.GetService<TEventHandler>();
        if (eventHandler == null)
        {
            throw new CouldNotInstantiateEventHandler(typeof(TEventHandler));
        }

        return await _method(eventHandler, typedEvent, context).TryTask().ConfigureAwait(false);
    }
}
