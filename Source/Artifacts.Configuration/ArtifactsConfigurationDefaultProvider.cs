// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.ApplicationModel;
using Dolittle.Configuration;

namespace Dolittle.Artifacts.Configuration
{
    /// <summary>
    /// Represents the <see cref="ICanProvideDefaultConfigurationFor{T}">default provider</see> for <see cref="ArtifactsConfiguration"/>.
    /// </summary>
    public class ArtifactsConfigurationDefaultProvider : ICanProvideDefaultConfigurationFor<ArtifactsConfiguration>
    {
        /// <inheritdoc/>
        public ArtifactsConfiguration Provide()
        {
            return new ArtifactsConfiguration(new Dictionary<Feature, ArtifactsByTypeDefinition>());
        }
    }
}