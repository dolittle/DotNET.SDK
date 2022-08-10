// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Autofac.Core;
using Autofac.Core.Activators.Delegate;
using Autofac.Core.Lifetime;
using Autofac.Core.Registration;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.SDK.DependencyInversion;

/// <summary>
/// Represents the <see cref="IRegistrationSource"/> for providing a registration source for unknown services on a tenant container.
/// </summary>
public class UnknownServiceOnTenantContainerRegistrationSource : IRegistrationSource
{
    const string MetadataKey = "from-dolittle-unknown-registration-source";
    readonly IServiceProvider _rootProvider;
    readonly IEnumerable<IComponentRegistration> _registrations;
    readonly bool _isTenantRootContainer;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnknownServiceOnTenantContainerRegistrationSource"/> class.
    /// </summary>
    /// <param name="rootProvider">The root <see cref="IServiceProvider"/>.s</param>
    /// <param name="registrations">The <see cref="IEnumerable{T}"/> of <see cref="ComponentRegistration"/>.</param>
    /// <param name="isTenantRootContainer">Whether the registration source belongs to the tenant root container.</param>
    public UnknownServiceOnTenantContainerRegistrationSource(IServiceProvider rootProvider, IEnumerable<IComponentRegistration> registrations, bool isTenantRootContainer)
    {
        _rootProvider = rootProvider;
        _registrations = registrations;
        _isTenantRootContainer = isTenantRootContainer;
    }

    /// <inheritdoc />
    public IEnumerable<IComponentRegistration> RegistrationsFor(Service service, Func<Service, IEnumerable<ServiceRegistration>> registrationAccessor)
    {
        if (service is not IServiceWithType serviceWithType)
        {
            return Enumerable.Empty<IComponentRegistration>();
        }
        if (RegisteredInContainer(service) || !IsRegisteredInRootContainer(serviceWithType.ServiceType))
        {
            return Enumerable.Empty<IComponentRegistration>();
        }

        var serviceType = serviceWithType.ServiceType;
        var registration = new ComponentRegistration(
            Guid.NewGuid(),
            new DelegateActivator(
                serviceType,
                (_, __) => _rootProvider.GetRequiredService(serviceType)),
            new CurrentScopeLifetime(),
            InstanceSharing.None,
            InstanceOwnership.ExternallyOwned,
            new[]
            {
                service
            },
            new Dictionary<string, object>
            {
                [MetadataKey] = null
            });
        return new[]
        {
            registration
        };
    }

    /// <inheritdoc />
    public bool IsAdapterForIndividualComponents => false;

    bool RegisteredInContainer(Service service)
    {
        foreach (var registration in _registrations)
        {
            if (registration.Services.Any(_ => _.Equals(service)))
            {
                return !registration.Metadata.ContainsKey(MetadataKey);
            }
        }
        return false;
    }
    
    bool IsRegisteredInRootContainer(Type service)
    {
        try
        {
            if (!_isTenantRootContainer)
            {
                return _rootProvider.GetService(service) is not null;
            }

            var scopeFactory = _rootProvider.GetService<IServiceScopeFactory>();
            if (scopeFactory is null)
            {
                return _rootProvider.GetService(service) is not null;
            }

            using var scope = scopeFactory.CreateScope();
            return scope.ServiceProvider.GetService(service) is not null;
        }
        catch
        {
            return false;
        }
    }
}
