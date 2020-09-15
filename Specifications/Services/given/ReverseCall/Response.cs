// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Services.Contracts;

namespace Dolittle.SDK.Services.given.ReverseCall
{
    public class Response
    {
        public Response(Request request)
        {
            Request = request;
        }

        public ReverseCallResponseContext Context { get; set; }

        public Request Request { get; }
    }
}