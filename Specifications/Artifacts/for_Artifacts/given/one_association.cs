// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.SDK.Artifacts.for_Artifacts.given;

public class one_association
{
    protected static Type associated_type;
    protected static artifact_type associated_artifact;
    protected static artifact_types artifacts;

    Establish context = () =>
    {
        associated_type = typeof(string);
        associated_artifact = new artifact_type("ba89fde2-e57a-4eba-8f9c-f20e2b021f82");
        artifacts = new artifact_types();
        artifacts.Add(associated_artifact, associated_type);
    };
}