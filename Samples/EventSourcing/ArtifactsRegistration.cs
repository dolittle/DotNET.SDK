// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Artifacts;
using Dolittle.Booting;

namespace EventSourcing
{
    public class ArtifactsRegistration : ICanRunBeforeBootStage<NoSettings>
    {
        public BootStage BootStage => BootStage.BootProcedures;

        public void Perform(NoSettings settings, IBootStageBuilder builder)
        {
            var artifactTypeMap = builder.Container.Get<IArtifactTypeMap>();
            artifactTypeMap.Register(new Artifact(Guid.Parse("ac14f174-572e-4c07-8cd1-e4e8ecc0fea9"), 1), typeof(MyAggregate));
            artifactTypeMap.Register(new Artifact(Guid.Parse("bc26f986-5515-4506-9944-cd7e93bec7fe"), 1), typeof(MyEvent));
            artifactTypeMap.Register(new Artifact(Guid.Parse("2626352e-41d2-4d40-b3dd-e8efdee9bcac"), 1), typeof(MySecondEvent));
            artifactTypeMap.Register(new Artifact(Guid.Parse("522c007d-9498-4aa2-a9ee-f43e8d5aec2a"), 1), typeof(MyCommand));
        }
    }
}