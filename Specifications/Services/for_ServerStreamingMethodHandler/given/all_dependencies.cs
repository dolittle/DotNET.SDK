// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.SDK.Services.given;
using Grpc.Core;
using Machine.Specifications;
using Status = Grpc.Core.Status;

namespace Dolittle.SDK.Services.for_ServerStreamingMethodHandler.given;

public class all_dependencies
{
    Establish context = () =>
    {
    };

    protected static ServerStreamingMethodHandler<Message> create_handler(FakeAsyncStreamReader<Message> reader)
        => new(new AsyncServerStreamingCall<Message>(reader, Task.FromResult(Metadata.Empty), () => Status.DefaultSuccess, () => Metadata.Empty, () => {}));
}