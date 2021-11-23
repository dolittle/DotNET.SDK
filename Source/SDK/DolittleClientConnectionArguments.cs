// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Globalization;
using System.Threading.Tasks;
using Dolittle.SDK.Execution;
using Dolittle.SDK.Microservices;
using Dolittle.SDK.Security;
using Dolittle.SDK.Tenancy;

namespace Dolittle.SDK
{
    /// <summary>
    /// Represents the arguments for connecting an <see cref="IDolittleClient"/>.
    /// </summary>
    public class DolittleClientConnectionArguments : ICanResolveExecutionContextForDolittleClient
    {
        /// <summary>
        /// Gets the <see cref="MicroserviceId"/>.
        /// </summary>
        public MicroserviceId Microservice { get; private set; } = MicroserviceId.NotSet;

        /// <summary>
        /// Gets the <see cref="Version"/>.
        /// </summary>
        public Version Version { get; private set; } = Version.NotSet;

        /// <summary>
        /// Gets the <see cref="Environment"/>.
        /// </summary>
        public Environment Environment { get; private set; } = Environment.Undetermined;

        /// <summary>
        /// Sets the id of the microservice.
        /// </summary>
        /// <param name="microservice">The id of the microservice.</param>
        /// <returns>The client builder for continuation.</returns>
        public DolittleClientConnectionArguments WithMicroservice(MicroserviceId microservice)
        {
            Microservice = microservice;
            return this;
        }

        /// <summary>
        /// Sets the version of the microservice.
        /// </summary>
        /// <param name="version">The version of the microservice.</param>
        /// <returns>The client builder for continuation.</returns>
        public DolittleClientConnectionArguments WithVersion(Version version)
        {
            Version = version;
            return this;
        }

        /// <summary>
        /// Sets the version of the microservice.
        /// </summary>
        /// <param name="major">Major version of the microservice.</param>
        /// <param name="minor">Minor version of the microservice.</param>
        /// <param name="patch">Path level of the microservice.</param>
        /// <param name="build">Build number of the microservice.</param>
        /// <param name="preReleaseString">If prerelease - the prerelease string.</param>
        /// <returns>The client builder for continuation.</returns>
        public DolittleClientConnectionArguments WithVersion(int major, int minor, int patch, int build = 0, string preReleaseString = "")
        {
            Version = new Version(major, minor, patch, build, preReleaseString);
            return this;
        }

        /// <summary>
        /// Sets the environment in which the microservice is running.
        /// </summary>
        /// <param name="environment">The environment in which the microservice is running.</param>
        /// <returns>The client builder for continuation.</returns>
        public DolittleClientConnectionArguments WithEnvironment(Environment environment)
        {
            Environment = environment;
            return this;
        }

        /// <inheritdoc />
        public Task<ExecutionContext> Resolve()
            => Task.FromResult(new ExecutionContext(
                Microservice,
                TenantId.System,
                Version,
                Environment,
                CorrelationId.System,
                Claims.Empty,
                CultureInfo.InvariantCulture));
    }
}