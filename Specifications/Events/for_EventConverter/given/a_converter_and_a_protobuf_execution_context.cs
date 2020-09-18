// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Execution.Contracts;
using Dolittle.SDK.Protobuf;
using Dolittle.Security.Contracts;
using Machine.Specifications;
using Version = Dolittle.Versioning.Contracts.Version;

namespace Dolittle.SDK.Events.for_EventConverter.given
{
    public class a_converter_and_a_protobuf_execution_context : a_converter
    {
        protected static ExecutionContext execution_context;

        Establish context = () =>
        {
            execution_context = new ExecutionContext
            {
                TenantId = Guid.Parse("a3f2c429-9365-4e3f-935b-e43af7f5cadc").ToProtobuf(),
                CorrelationId = Guid.Parse("c2f187b1-f665-4d43-97a0-ec0196de78dd").ToProtobuf(),
                MicroserviceId = Guid.Parse("6b31e2eb-f792-4f71-bc7d-99ba02fafa21").ToProtobuf(),
                Environment = "giwhiwarof",
                Version = new Version
                {
                    Major = 13,
                    Minor = 7,
                    Patch = 5,
                    Build = 1001,
                    PreReleaseString = "tahpigheze",
                },
            };
            execution_context.Claims.Add(new Claim
                {
                    Key = "dijjufpubu",
                    Value = "heletfokha",
                    ValueType = "comisihsoc"
                });
            execution_context.Claims.Add(new Claim
                {
                    Key = "taudpusiwadapek",
                    Value = "nalfa",
                    ValueType = "azjetmunigpuavwomuze"
                });
        };
    }
}
