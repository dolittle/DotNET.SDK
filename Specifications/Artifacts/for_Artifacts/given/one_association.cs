// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Machine.Specifications;
using Microsoft.Extensions.Logging;
using Moq;

namespace Dolittle.SDK.Artifacts.for_Artifacts.given
{
    public class one_association
    {
        protected static Type associated_type;
        protected static Artifact associated_artifact;
        protected static Artifacts artifacts;

        Establish context = () =>
        {
            associated_type = typeof(string);
            associated_artifact = new artifact_type("ba89fde2-e57a-4eba-8f9c-f20e2b021f82");
            artifacts = new Artifacts(
                new Dictionary<Type, Artifact>()
                {
                    { associated_type, associated_artifact }
                },
                Mock.Of<ILogger<Artifacts>>());
        };
    }
}