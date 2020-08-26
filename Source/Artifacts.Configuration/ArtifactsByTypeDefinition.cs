// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Dolittle.Artifacts.Configuration
{
    /// <summary>
    /// Represents the definition of artifacts grouped by the different types for configuration.
    /// </summary>
    public class ArtifactsByTypeDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArtifactsByTypeDefinition"/> class.
        /// </summary>
        /// <param name="commands">Command artifacts.</param>
        /// <param name="events">Event artifacts.</param>
        /// <param name="eventSources">EventSource artifacts.</param>
        /// <param name="readModels">ReadModel artifacts.</param>
        /// <param name="queries">Query artifacts.</param>
        public ArtifactsByTypeDefinition(
            IDictionary<ArtifactId, ArtifactDefinition> commands,
            IDictionary<ArtifactId, ArtifactDefinition> events,
            IDictionary<ArtifactId, ArtifactDefinition> eventSources,
            IDictionary<ArtifactId, ArtifactDefinition> readModels,
            IDictionary<ArtifactId, ArtifactDefinition> queries)
        {
            Commands = new ReadOnlyDictionary<ArtifactId, ArtifactDefinition>(commands);
            Events = new ReadOnlyDictionary<ArtifactId, ArtifactDefinition>(events);
            EventSources = new ReadOnlyDictionary<ArtifactId, ArtifactDefinition>(eventSources);
            ReadModels = new ReadOnlyDictionary<ArtifactId, ArtifactDefinition>(readModels);
            Queries = new ReadOnlyDictionary<ArtifactId, ArtifactDefinition>(queries);
        }

        /// <summary>
        /// Gets the Command artifacts.
        /// </summary>
        public IReadOnlyDictionary<ArtifactId, ArtifactDefinition> Commands { get; }

        /// <summary>
        /// Gets the Event artifacts.
        /// </summary>
        public IReadOnlyDictionary<ArtifactId, ArtifactDefinition> Events { get; }

        /// <summary>
        /// Gets the EventSource artifacts.
        /// </summary>
        public IReadOnlyDictionary<ArtifactId, ArtifactDefinition> EventSources { get; }

        /// <summary>
        /// Gets the ReadModel artifacts.
        /// </summary>
        public IReadOnlyDictionary<ArtifactId, ArtifactDefinition> ReadModels { get; }

        /// <summary>
        /// Gets the Query artifacts.
        /// </summary>
        public IReadOnlyDictionary<ArtifactId, ArtifactDefinition> Queries { get; }
    }
}