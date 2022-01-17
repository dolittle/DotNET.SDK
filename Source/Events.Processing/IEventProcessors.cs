// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.SDK.Concepts;
using Dolittle.SDK.Events.Processing.Internal;
using Dolittle.SDK.Services;
using Google.Protobuf;

namespace Dolittle.SDK.Events.Processing;

/// <summary>
/// Defines a system that handles the registration of event processor.
/// </summary>
public interface IEventProcessors
{
    /// <summary>
    /// Register an <see cref="IEventProcessor{TIdentifier, TRegisterResponse, TRequest, TResponse}" />.
    /// </summary>
    /// <param name="eventProcessor">The <see cref="IEventProcessor{TIdentifier, TRegisterResponse, TRequest, TResponse}" /> to register.</param>
    /// <param name="protocol">The <see cref="IAmAReverseCallProtocol{TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}"/>.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <typeparam name="TIdentifier">The <see cref="Type" /> of the <see cref="ConceptAs{T}" /> <see cref="Guid" />.>.</typeparam>
    /// <typeparam name="TClientMessage">The <see cref="Type" /> of the reverse call client message.</typeparam>
    /// <typeparam name="TServerMessage">The <see cref="Type" /> of the reverse call client server message.</typeparam>
    /// <typeparam name="TRegisterArguments">The <see cref="Type" /> of the registration arguments.</typeparam>
    /// <typeparam name="TRegisterResponse">The <see cref="Type" /> of the registration response.</typeparam>
    /// <typeparam name="TRequest">The <see cref="Type" /> of the request.</typeparam>
    /// <typeparam name="TResponse">The <see cref="Type" /> of the response.</typeparam>
    void Register<TIdentifier, TClientMessage, TServerMessage, TRegisterArguments, TRegisterResponse, TRequest, TResponse>(
        IEventProcessor<TIdentifier, TRegisterArguments, TRequest,  TResponse> eventProcessor,
        IAmAReverseCallProtocol<TClientMessage, TServerMessage, TRegisterArguments, TRegisterResponse, TRequest, TResponse> protocol,
        CancellationToken cancellationToken)
        where TIdentifier : ConceptAs<Guid>
        where TClientMessage : class, IMessage
        where TServerMessage : class, IMessage
        where TRegisterArguments : class
        where TRegisterResponse : class
        where TRequest : class
        where TResponse : class;
}
