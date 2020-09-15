// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Execution;
using Google.Protobuf;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Services
{
    /// <summary>
    /// An implementation of <see cref="ICreateReverseCallClients"/>.
    /// </summary>
    public class ReverseCallClientCreator : ICreateReverseCallClients
    {
        readonly TimeSpan _pingInterval;
        readonly IPerformMethodCalls _caller;
        readonly IExecutionContextManager _executionContextManager;
        readonly ILoggerFactory _loggerFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReverseCallClientCreator"/> class.
        /// </summary>
        /// <param name="pingInterval">The interval at which to request pings from the server to keep the reverse calls alive.</param>
        /// <param name="caller">The caller that will be used to perform the method calls.</param>
        /// <param name="executionContextManager">The execution context manager that will be used to set the execution context while handling requests from the server.</param>
        /// <param name="loggerFactory">The logger that will be used create loggers to log messages while performing the reverse call.</param>
        public ReverseCallClientCreator(
            TimeSpan pingInterval,
            IPerformMethodCalls caller,
            IExecutionContextManager executionContextManager,
            ILoggerFactory loggerFactory)
        {
            _pingInterval = pingInterval;
            _caller = caller;
            _executionContextManager = executionContextManager;
            _loggerFactory = loggerFactory;
        }

        /// <inheritdoc/>
        public IReverseCallClient<TConnectArguments, TConnectResponse, TRequest, TResponse> Create<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>(TConnectArguments arguments, IReverseCallHandler<TRequest, TResponse> handler, ICanCallADuplexStreamingMethod<TClientMessage, TServerMessage> method, IConvertReverseCallMessages<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> converter)
            where TClientMessage : IMessage
            where TServerMessage : IMessage
            where TConnectArguments : class
            where TConnectResponse : class
            where TRequest : class
            where TResponse : class
            => new ReverseCallClient<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>(
                arguments,
                handler,
                method,
                converter,
                _pingInterval,
                _caller,
                _executionContextManager,
                _loggerFactory.CreateLogger<ReverseCallClient<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>>());
    }
}