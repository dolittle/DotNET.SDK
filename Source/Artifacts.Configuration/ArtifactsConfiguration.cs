// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Dolittle.ApplicationModel;
using Dolittle.Configuration;

namespace Dolittle.Artifacts.Configuration
{
    /// <summary>
    /// Represents the definition of features for configuration.
    /// </summary>
    [Name("artifacts")]
    public class ArtifactsConfiguration :
        ReadOnlyDictionary<Feature, ArtifactsByTypeDefinition>,
        IConfigurationObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArtifactsConfiguration"/> class.
        /// </summary>
        /// <param name="artifacts"><see cref="IDictionary{TKey, TValue}"/> for artifacts per feature.</param>
        public ArtifactsConfiguration(IDictionary<Feature, ArtifactsByTypeDefinition> artifacts)
            : base(artifacts)
        {
        }
    }
}