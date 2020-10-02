// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reactive.Concurrency;
using System.Threading;
using Google.Protobuf;
using Microsoft.Extensions.Logging;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;

namespace Dolittle.SDK.Services
{
    /// <summary>
    /// An implementation of <see cref="ICreateReverseCallClients"/>.
    /// </summary>
    public class ReverseCallClientCreator : ICreateReverseCallClients
    {
        static readonly IScheduler _scheduler = TaskPoolScheduler.Default;
        readonly TimeSpan _pingInterval;
        readonly IPerformMethodCalls _caller;
        readonly ExecutionContext _executionContext;
        readonly ILoggerFactory _loggerFactory;
        readonly CancellationToken _cancellationToken;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReverseCallClientCreator"/> class.
        /// </summary>
        /// <param name="pingInterval">The interval at which to request pings from the server to keep the reverse calls alive.</param>
        /// <param name="caller">The caller that will be used to perform the method calls.</param>
        /// <param name="executionContext">The execution context to use while initiating reverse calls.</param>
        /// <param name="loggerFactory">The logger that will be used create loggers to log messages while performing the reverse call.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to cancel the call.</param>
        public ReverseCallClientCreator(
            TimeSpan pingInterval,
            IPerformMethodCalls caller,
            ExecutionContext executionContext,
            ILoggerFactory loggerFactory,
            CancellationToken cancellationToken)
        {
            _pingInterval = pingInterval;
            _caller = caller;
            _executionContext = executionContext;
            _loggerFactory = loggerFactory;
            _cancellationToken = cancellationToken;
        }

        /// <inheritdoc/>
        public IReverseCallClient<TConnectArguments, TConnectResponse, TRequest, TResponse> Create<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>(
            TConnectArguments arguments,
            IReverseCallHandler<TRequest, TResponse> handler,
            IAmAReverseCallProtocol<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> protocol)
            where TClientMessage : class, IMessage
            where TServerMessage : class, IMessage
            where TConnectArguments : class
            where TConnectResponse : class
            where TRequest : class
            where TResponse : class
            => new ReverseCallClient<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>(
                arguments,
                handler,
                protocol,
                _pingInterval,
                _caller,
                _executionContext,
                _scheduler,
                _loggerFactory.CreateLogger<ReverseCallClient<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>>(),
                _cancellationToken);
    }
}
