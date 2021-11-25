// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Autofac;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.SDK.DependencyInversion
{
    /// <summary>
    /// Represents an implementation of <see cref="IServiceScopeFactory"/> that creates an <see cref="IServiceScope"/>
    /// that manages the scopes of both the root <see cref="IServiceProvider"/> and the tenant specific <see cref="IServiceProvider"/>.
    /// </summary>
    public class ServiceScopeFactory : IServiceScopeFactory
    {
        readonly IServiceScopeFactory _rootServiceScopeFactory;
        readonly ILifetimeScope _lifetimeScope;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceScopeFactory"/> class.
        /// </summary>
        /// <param name="rootServiceScopeFactory">The root <see cref="IServiceScopeFactory"/>.</param>
        /// <param name="lifetimeScope">The <see cref="ILifetimeScope"/>.</param>
        public ServiceScopeFactory(IServiceScopeFactory rootServiceScopeFactory, ILifetimeScope lifetimeScope)
        {
            _rootServiceScopeFactory = rootServiceScopeFactory;
            _lifetimeScope = lifetimeScope;
        }

        /// <inheritdoc />
        public IServiceScope CreateScope()
        {
            Console.WriteLine("Creating scope");
            var rootScope = _rootServiceScopeFactory.CreateScope();
            var tenantProviderScope = _lifetimeScope.BeginLifetimeScope(builder =>
                builder.RegisterSource(
                    new UnknownServiceOnTenantContainerRegistrationSource(rootScope.ServiceProvider)));
            return new TenantServiceScope(rootScope, tenantProviderScope);
        }
    }
}