// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.ApplicationModel;
using Dolittle.DependencyInversion;

namespace Dolittle.Microservice.Configuration
{
    /// <summary>
    /// Binds up the bindings related to the running application. The<see cref="Application"/>, the <see cref="ApplicationModel.Microservice"/> and the <see cref="MicroserviceName"/>.
    /// </summary>
    public class Bindings : ICanProvideBindings
    {
        readonly MicroserviceConfiguration _boundedContextConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="Bindings"/> class.
        /// </summary>
        /// <param name="microserviceConfiguration"><see cref="MicroserviceConfiguration">Configuration</see> for the microservice.</param>
        public Bindings(MicroserviceConfiguration microserviceConfiguration)
        {
            _boundedContextConfiguration = microserviceConfiguration;
        }

        /// <inheritdoc/>
        public void Provide(IBindingProviderBuilder builder)
        {
            builder.Bind<Application>().To(() => _boundedContextConfiguration.Application);
            builder.Bind<ApplicationModel.Microservice>().To(() => _boundedContextConfiguration.Microservice);
            builder.Bind<MicroserviceName>().To(() => _boundedContextConfiguration.MicroserviceName);
        }
    }
}