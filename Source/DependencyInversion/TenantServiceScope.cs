// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.SDK.DependencyInversion
{
    /// <summary>
    /// Represents an implementation of <see cref="IServiceScope"/> that uses both the scope for the root <see cref="IServiceProvider"/> and the tenant specific <see cref="IServiceProvider"/>.
    /// </summary>
    public class TenantServiceScope : IServiceScope
    {
        readonly IServiceScope _rootScope;
        readonly ILifetimeScope _tenantScope;

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantServiceScope"/> class.
        /// </summary>
        /// <param name="rootScope">The <see cref="IServiceScope"/> root scope.</param>
        /// <param name="tenantScope">The <see cref="ILifetimeScope"/> tenant service scope.</param>
        public TenantServiceScope(IServiceScope rootScope, ILifetimeScope tenantScope)
        {
            _rootScope = rootScope;
            _tenantScope = tenantScope;
            ServiceProvider = new AutofacServiceProvider(tenantScope);
        }

        /// <inheritdoc />
        public IServiceProvider ServiceProvider { get; }

        /// <inheritdoc />
        public void Dispose()
        {
            _rootScope?.Dispose();
            _tenantScope?.Dispose();
        }
    }
}