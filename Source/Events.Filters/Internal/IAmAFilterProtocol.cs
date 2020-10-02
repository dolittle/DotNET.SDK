// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.SDK.Services;
using Google.Protobuf;

namespace Dolittle.SDK.Events.Filters.Internal
{
    /// <summary>
    /// Defines a <see cref="IAmAReverseCallProtocol{TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}" /> for filters.
    /// </summary>
    /// <typeparam name="TClientMessage">The <see cref="System.Type" /> of the client message.</typeparam>
    /// <typeparam name="TRegisterArguments">The <see cref="System.Type" /> of the registration arguments.</typeparam>
    /// <typeparam name="TResponse">The <see cref="System.Type" /> of the response.</typeparam>
    public interface IAmAFilterProtocol<TClientMessage, TRegisterArguments, TResponse> : IAmAReverseCallProtocol<TClientMessage, FilterRuntimeToClientMessage, TRegisterArguments, FilterRegistrationResponse, FilterEventRequest, TResponse>
        where TClientMessage : IMessage
        where TRegisterArguments : class
        where TResponse : class
    {
    }
}