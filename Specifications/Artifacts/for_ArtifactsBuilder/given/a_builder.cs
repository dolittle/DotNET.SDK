// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Artifacts.for_ArtifactsBuilder.given
{
    public class a_builder
    {
        protected static ArtifactsBuilder builder;

        Establish context = () =>
        {
            var logger_factory = new LoggerFactory();

            builder = new ArtifactsBuilder(logger_factory);
        };
    }
}