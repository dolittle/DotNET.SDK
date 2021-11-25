// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Autofac;
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
        readonly ObjectIDGenerator _idGenerator;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownServiceOnTenantContainerRegistrationSource"/> class.
        /// </summary>
        /// <param name="rootProvider"></param>
        public UnknownServiceOnTenantContainerRegistrationSource(IServiceProvider rootProvider)
        {
            _rootProvider = rootProvider;
            _idGenerator = new ObjectIDGenerator();
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

            var registration = new ComponentRegistration(
                Guid.NewGuid(),
                new DelegateActivator(
                    serviceWithType.ServiceType,
                    (_, __) => _rootProvider
                        .GetRequiredService(serviceWithType.ServiceType)),
                new CurrentScopeLifetime(),
                InstanceSharing.None,
                InstanceOwnership.OwnedByLifetimeScope,
                new[] { service },
                new Dictionary<string, object>());
            // var registration = new ComponentRegistration(
            //     Guid.NewGuid(),
            //     new InstanceActivator(serviceWithType.ServiceType),
            //     new CurrentScopeLifetime(),
            //     InstanceSharing.None,
            //     InstanceOwnership.OwnedByLifetimeScope,
            //     new[] { service },
            //     new Dictionary<string, object>());
            return new[] { registration };
        }

        /// <inheritdoc />
        public bool IsAdapterForIndividualComponents => false;
    }
}