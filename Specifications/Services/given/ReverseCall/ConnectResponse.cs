// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Protobuf.Contracts;

namespace Dolittle.SDK.Services.given.ReverseCall
{
    public class ConnectResponse
    {
        public ConnectResponse(ConnectArguments arguments)
        {
            Arguments = arguments;
        }

        public ConnectArguments Arguments { get; }

        public Failure Failure { get; }
    }
}