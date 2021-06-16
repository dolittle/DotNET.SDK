// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Services
{
    /// <summary>
    /// Exception that gets thrown when <see cref="ReverseCallClient{TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}.Handle(IReverseCallHandler{TRequest, TResponse}, System.Threading.CancellationToken)" />
    /// is called more than once.
    /// </summary>
    public class ReverseCallClientAlreadyStartedHandling : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReverseCallClientAlreadyStartedHandling"/> class.
        /// </summary>
        public ReverseCallClientAlreadyStartedHandling()
            : base("Reverse Call Client can only start handling once")
        {
        }
    }
}