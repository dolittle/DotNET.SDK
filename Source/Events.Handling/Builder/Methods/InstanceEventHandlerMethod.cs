// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.SDK.Async;

namespace Dolittle.SDK.Events.Handling.Builder.Methods;

/// <summary>
/// An implementation of <see cref="IEventHandlerMethod" /> that invokes a method on an event handler instance for an event of a specific type.
/// </summary>
/// <typeparam name="TEventHandler">The <see cref="Type" /> of the event handler.</typeparam>
public class InstanceEventHandlerMethod<TEventHandler> : IEventHandlerMethod
    where TEventHandler : class
{
    readonly TEventHandler _instance;
    readonly TaskEventHandlerMethodSignature<TEventHandler> _method;

    /// <summary>
    /// Initializes a new instance of the <see cref="InstanceEventHandlerMethod{TEventHandler}"/> class.
    /// </summary>
    /// <param name="instance">The instance of the event handler.</param>
    /// <param name="method">The <see cref="TaskEventHandlerMethodSignature{TEvent}"/> method to invoke.</param>
    public InstanceEventHandlerMethod(TEventHandler instance, TaskEventHandlerMethodSignature<TEventHandler> method)
    {
        _instance = instance;
        _method = method;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InstanceEventHandlerMethod{TEventHandler}"/> class.
    /// </summary>
    /// <param name="instance">The instance of the event handler.</param>
    /// <param name="method">The <see cref="VoidEventHandlerMethodSignature{TEvent}" /> method to invoke.</param>
    public InstanceEventHandlerMethod(TEventHandler instance, VoidEventHandlerMethodSignature<TEventHandler> method)
        : this(
            instance,
            (instance, @event, context) =>
            {
                method(instance, @event, context);
                return Task.CompletedTask;
            })
    {
    }

    /// <inheritdoc/>
    public Task<Try> TryHandle(object @event, EventContext context, IServiceProvider serviceProvider)
        => _method(_instance, @event, context).TryTask();
}
