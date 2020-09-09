// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.SDK.Artifacts.given.ReverseCall
{
    public class ConnectResponse
    {
        public ConnectResponse(ConnectArguments arguments)
        {
            Arguments = arguments;
        }

        public ConnectArguments Arguments { get; }
    }
}