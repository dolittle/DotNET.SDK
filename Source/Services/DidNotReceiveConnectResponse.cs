// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Services
{
    /// <summary>
    /// Exception that gets thrown when the server does not send a connect response message as the first message while performing a reverse call.
    /// </summary>
    public class DidNotReceiveConnectResponse : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DidNotReceiveConnectResponse"/> class.
        /// </summary>
        public DidNotReceiveConnectResponse()
            : base("The server did not respond with a connect response message as the first message.")
        {
        }
    }
}