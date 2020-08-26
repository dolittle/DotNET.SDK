// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.ApplicationModel;
using Dolittle.Configuration;
using Dolittle.ResourceTypes;

namespace Dolittle.Microservice.Configuration
{
    /// <summary>
    /// Represents the definition of a <see cref="ApplicationModel.Microservice"/> for configuration.
    /// </summary>
    [Name("microservice")]
    public class MicroserviceConfiguration : IConfigurationObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MicroserviceConfiguration"/> class.
        /// </summary>
        /// <param name="application"><see cref="Application"/> this belongs to.</param>
        /// <param name="microservice"><see cref="ApplicationModel.Microservice"/> running.</param>
        /// <param name="microserviceName"><see cref="ApplicationModel.MicroserviceName" /> of bounded context.</param>
        /// <param name="core">The <see cref="CoreConfiguration"/>.</param>
        /// <param name="interaction">The <see cref="InteractionLayerConfiguration"/>.</param>
        /// <param name="resources">Resource configurations for different types.</param>
        public MicroserviceConfiguration(
            Application application,
            ApplicationModel.Microservice microservice,
            MicroserviceName microserviceName,
            CoreConfiguration core,
            IEnumerable<InteractionLayerConfiguration> interaction,
            IDictionary<ResourceType, ResourceTypeImplementationConfiguration> resources)
        {
            Application = application;
            Microservice = microservice;
            MicroserviceName = microserviceName;
            Core = core;
            Interaction = interaction;
            Resources = resources;
        }

        /// <summary>
        /// Gets the <see cref="Application"/>.
        /// </summary>
        public Application Application { get; }

        /// <summary>
        /// Gets the <see cref="ApplicationModel.Microservice"/>.
        /// </summary>
        public ApplicationModel.Microservice Microservice { get; }

        /// <summary>
        /// Gets the <see cref="ApplicationModel.MicroserviceName"/>.
        /// </summary>
        public MicroserviceName MicroserviceName { get; }

        /// <summary>
        /// Gets the <see cref="CoreConfiguration"/>.
        /// </summary>
        public CoreConfiguration Core { get; }

        /// <summary>
        /// Gets the <see cref="InteractionLayerConfiguration"/> list.
        /// </summary>
        public IEnumerable<InteractionLayerConfiguration> Interaction { get; }

        /// <summary>
        /// Gets the Resource configurations.
        /// </summary>
        public IDictionary<ResourceType, ResourceTypeImplementationConfiguration> Resources { get; }
    }
}