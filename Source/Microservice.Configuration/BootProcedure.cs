// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Dolittle.Booting;
using Dolittle.Execution;
using Dolittle.ResourceTypes.Configuration;
using Dolittle.Versioning;

namespace Dolittle.Microservice.Configuration
{
    /// <summary>
    /// Performs the boot procedures for the application configuration.
    /// </summary>
    public class BootProcedure : ICanPerformBootProcedure
    {
        readonly IExecutionContextManager _executionContextManager;
        readonly IResourceConfiguration _resourceConfiguration;
        readonly MicroserviceConfiguration _microserviceConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="BootProcedure"/> class.
        /// </summary>
        /// <param name="microserviceConfiguration"><see cref="MicroserviceConfiguration"/> to use.</param>
        /// <param name="executionContextManager"><see cref="IExecutionContextManager"/> to use for <see cref="ExecutionContext"/>.</param>
        /// <param name="resourceConfiguration"><see cref="IResourceConfiguration">Configuration</see> of resources.</param>
        public BootProcedure(
            MicroserviceConfiguration microserviceConfiguration,
            IExecutionContextManager executionContextManager,
            IResourceConfiguration resourceConfiguration)
        {
            _executionContextManager = executionContextManager;
            _resourceConfiguration = resourceConfiguration;
            _microserviceConfiguration = microserviceConfiguration;
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
            var environment = _executionContextManager.Current.Environment;

            _resourceConfiguration.ConfigureResourceTypes(
                _microserviceConfiguration.Resources.ToDictionary(
                    kvp => kvp.Key,
                    kvp => environment == Environment.Production ? kvp.Value.Production : kvp.Value.Development));

            _executionContextManager.SetConstants(_microserviceConfiguration.Microservice, Version.NotSet, environment);
            _executionContextManager.CurrentFor(_microserviceConfiguration.Microservice, _executionContextManager.Current.Tenant);

            HasPerformed = true;
        }
    }
}