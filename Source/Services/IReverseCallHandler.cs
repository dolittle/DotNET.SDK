// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Tenancy;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;

namespace Dolittle.SDK.Services;

/// <summary>
/// Defines a handler for handling reverse calls coming from the server to the client.
/// </summary>
/// <typeparam name="TRequest">Type of the requests sent from the server to the client.</typeparam>
/// <typeparam name="TResponse">Type of the responses received from the client.</typeparam>
public interface IReverseCallHandler<TRequest, TResponse>
    where TRequest : class
    where TResponse : class
{
    /// <summary>
    /// Method that will be called to handle requests coming from the server.
    /// </summary>
    /// <param name="request">The request to handle.</param>
    /// <param name="executionContext">The execution context to handle the request in.</param>
    /// <param name="serviceProvider">The <see cref="IServiceProvider"/> for the <see cref="TenantId"/> in the <see cref="ExecutionContext"/>.</param>
    /// <param name="token">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task"/> that, when resolved, returns the <typeparamref name="TResponse"/> to send to the server.</returns>
    Task<TResponse> Handle(TRequest request, ExecutionContext executionContext, IServiceProvider serviceProvider, CancellationToken token);
}
