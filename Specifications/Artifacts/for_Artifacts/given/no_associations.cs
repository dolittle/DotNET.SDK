// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Machine.Specifications;
using Microsoft.Extensions.Logging;
using Moq;

namespace Dolittle.SDK.Artifacts.for_ArtifactTypeMap.given
{
    public class no_associations
    {
        protected static Artifacts artifacts;

        Establish context = () => artifacts = new Artifacts(new Dictionary<Type, Artifact>(), Mock.Of<ILogger<Artifacts>>());
    }
}