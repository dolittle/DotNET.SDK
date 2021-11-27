// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Autofac.Core;
using Autofac.Core.Activators.Delegate;
using Autofac.Core.Lifetime;
using Autofac.Core.Registration;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.SDK.DependencyInversion
{
    /// <summary>
    /// Represents the <see cref="IRegistrationSource"/> for providing a registration source for unknown services on a tenant container.
    /// </summary>
    public class UnknownServiceOnTenantContainerRegistrationSource : IRegistrationSource
    {
        readonly IServiceProvider _rootProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownServiceOnTenantContainerRegistrationSource"/> class.
        /// </summary>
        /// <param name="rootProvider">The root <see cref="IServiceProvider"/>.s</param>
        public UnknownServiceOnTenantContainerRegistrationSource(IServiceProvider rootProvider)
        {
            _rootProvider = rootProvider;
        }

        /// <inheritdoc />
        public IEnumerable<IComponentRegistration> RegistrationsFor(Service service, Func<Service, IEnumerable<ServiceRegistration>> registrationAccessor)
        {
            if (!(service is IServiceWithType serviceWithType)
                || registrationAccessor(service).Any()
                || _rootProvider.GetService(serviceWithType.ServiceType) == null)
            {
                return Enumerable.Empty<IComponentRegistration>();
            }
            var serviceType = serviceWithType.ServiceType;

            var registration = new ComponentRegistration(
                Guid.NewGuid(),
                new DelegateActivator(
                    serviceType,
                    (_, __) => _rootProvider
                        .GetRequiredService(serviceType)),
                new CurrentScopeLifetime(),
                InstanceSharing.None,
                InstanceOwnership.ExternallyOwned,
                new[] { service },
                new Dictionary<string, object>());
            return new[] { registration };
        }

        /// <inheritdoc />
        public bool IsAdapterForIndividualComponents => false;
    }
}