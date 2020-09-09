// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Artifacts.given.ReverseCall;
using Dolittle.SDK.Services;

namespace Dolittle.SDK.Artifacts.given
{
    public class Handler : IReverseCallHandler<Request, Response>
    {
        public Task<Response> Handle(Request request, CancellationToken token)
        {
            return Task.FromResult(new Response(request));
        }
    }
}