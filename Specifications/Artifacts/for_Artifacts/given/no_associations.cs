// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.SDK.Artifacts.for_Artifacts.given
{
    public class no_associations
    {
        protected static artifact_types artifacts;

        Establish context = () => artifacts = new artifact_types();
    }
}