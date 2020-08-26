// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dolittle.Booting;
using Dolittle.Collections;

namespace Dolittle.Artifacts.Configuration
{
    /// <summary>
    /// Represents the <see cref="ICanPerformBootProcedure">boot procedure</see> for artifacts.
    /// </summary>
    public class BootProcedure : ICanPerformBootProcedure
    {
        readonly IArtifactTypeMap _artifactTypeMap;

        readonly IEnumerable<PropertyInfo> _artifactProperties = typeof(ArtifactsByTypeDefinition).GetProperties().Where(_ => _.PropertyType == typeof(IReadOnlyDictionary<ArtifactId, ArtifactDefinition>));
        readonly ArtifactsConfiguration _artifacts;

        /// <summary>
        /// Initializes a new instance of the <see cref="BootProcedure"/> class.
        /// </summary>
        /// <param name="artifactsConfiguration">The <see cref="ArtifactsConfiguration"/>.</param>
        /// <param name="artifactTypeMap">The <see cref="IArtifactTypeMap"/>.</param>
        public BootProcedure(ArtifactsConfiguration artifactsConfiguration, IArtifactTypeMap artifactTypeMap)
        {
            _artifacts = artifactsConfiguration;
            _artifactTypeMap = artifactTypeMap;
        }

        /// <summary>
        /// Gets a value indicating whether or not this <see cref="ICanPerformBootProcedure">boot procedure</see> has performed.
        /// </summary>
        public static bool HasPerformed { get; private set; }

        /// <inheritdoc/>
        public bool CanPerform() => true;

        /// <inheritdoc/>
        public void Perform()
        {
            _artifacts.Select(_ => _.Value).ForEach(artifactByType =>
            {
                _artifactProperties.ForEach(property =>
                {
                    var artifacts = property.GetValue(artifactByType) as IReadOnlyDictionary<ArtifactId, ArtifactDefinition>;
                    artifacts.ForEach(artifactDefinitionEntry => _artifactTypeMap.Register(
                        new Artifact(artifactDefinitionEntry.Key, artifactDefinitionEntry.Value.Generation),
                        artifactDefinitionEntry.Value.Type.GetActualType()));
                });
            });

            HasPerformed = true;
        }
    }
}