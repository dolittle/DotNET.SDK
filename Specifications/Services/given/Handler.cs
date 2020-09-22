// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Services.given.ReverseCall;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;

namespace Dolittle.SDK.Services.given
{
    public class Handler : IReverseCallHandler<Request, Response>
    {
        public Task<Response> Handle(Request request, ExecutionContext executionContext, CancellationToken token)
        {
            return Task.FromResult(new Response(request));
        }
    }
}