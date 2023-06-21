// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.DependencyInversion;
using Dolittle.SDK.Tenancy;
using Google.Protobuf;
using Microsoft.Extensions.Logging;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;

namespace Dolittle.SDK.Services;

/// <summary>
/// An implementation of <see cref="ICreateReverseCallClients"/>.
/// </summary>
public class ReverseCallClientCreator : ICreateReverseCallClients
{
    readonly TimeSpan _pingInterval;
    readonly IPerformMethodCalls _caller;
    readonly ExecutionContext _executionContext;
    readonly ITenantScopedProviders _tenantScopedProviders;
    readonly ILoggerFactory _loggerFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReverseCallClientCreator"/> class.
    /// </summary>
    /// <param name="pingInterval">The interval at which to request pings from the server to keep the reverse calls alive.</param>
    /// <param name="caller">The caller that will be used to perform the method calls.</param>
    /// <param name="executionContext">The execution context to use while initiating reverse calls.</param>
    /// <param name="tenantScopedProviders">The <see cref="ITenantScopedProviders"/> for resolving a <see cref="IServiceProvider"/> for a specific <see cref="TenantId"/>.</param>
    /// <param name="loggerFactory">The logger that will be used create loggers to log messages while performing the reverse call.</param>
    public ReverseCallClientCreator(
        TimeSpan pingInterval,
        IPerformMethodCalls caller,
        ExecutionContext executionContext,
        ITenantScopedProviders tenantScopedProviders,
        ILoggerFactory loggerFactory)
    {
        _pingInterval = pingInterval;
        _caller = caller;
        _executionContext = executionContext;
        _tenantScopedProviders = tenantScopedProviders;
        _loggerFactory = loggerFactory;
    }

    /// <inheritdoc/>
    public IReverseCallClient<TConnectArguments, TConnectResponse, TRequest, TResponse, TClientMessage> Create<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>(
        IAmAReverseCallProtocol<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> protocol)
        where TClientMessage : class, IMessage
        where TServerMessage : class, IMessage
        where TConnectArguments : class
        where TConnectResponse : class
        where TRequest : class
        where TResponse : class
        => new ReverseCallClient<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>(
            protocol,
            _pingInterval,
            _caller,
            _executionContext,
            _tenantScopedProviders,
            _loggerFactory.CreateLogger<ReverseCallClient<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>>());
}
