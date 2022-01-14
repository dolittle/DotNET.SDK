// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.SDK.Async;
using Dolittle.SDK.Tenancy;

namespace Dolittle.SDK.Events.Handling.Builder.Methods;

/// <summary>
/// Defines an event handler method.
/// </summary>
public interface IEventHandlerMethod
{
    /// <summary>
    /// Invokes the event handler method.
    /// </summary>
    /// <param name="event">The event.</param>
    /// <param name="context">The <see cref="EventContext" />.</param>
    /// <param name="serviceProvider">The <see cref="IServiceProvider"/> for the <see cref="TenantId"/> in the <see cref="EventContext.CurrentExecutionContext"/>.</param>
    /// <returns>A <see cref="Task" /> that, when resolved, returns <see cref="Try" />.</returns>
    Task<Try> TryHandle(object @event, EventContext context, IServiceProvider serviceProvider);
}
