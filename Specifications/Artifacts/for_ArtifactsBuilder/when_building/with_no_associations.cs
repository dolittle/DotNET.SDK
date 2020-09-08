// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.SDK.Artifacts.for_ArtifactsBuilder.when_building
{
    public class with_no_associations : given.a_builder
    {
        static IArtifacts results;

        Because of = () => results = builder.Build();

        It should_return_an_instance = () => results.ShouldNotBeNull();
    }
}