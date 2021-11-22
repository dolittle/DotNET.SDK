// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Execution;
using Dolittle.SDK.Tenancy;

namespace Dolittle.SDK.DependencyInversion
{
    /// <summary>
    /// Defines an IoC container for dependency inversion.
    /// </summary>
    public interface IContainer : IServiceProvider
    {
        /// <summary>
        /// Gets the instance of a service.
        /// </summary>
        /// <param name="service">The <see cref="Type" /> of service to get.</param>
        /// <param name="context">The <see cref="ExecutionContext"/> to use while resolving the dependency.</param>
        /// <returns>The instance.</returns>
        object Get(Type service, ExecutionContext context);

        /// <summary>
        /// Gets the instance of a service.
        /// </summary>
        /// <param name="service">The <see cref="Type" /> of service to get.</param>
        /// <param name="tenant">The <see cref="TenantId"/> to use while resolving the dependency.</param>
        /// <returns>The instance.</returns>
        object Get(Type service, TenantId tenant);

        /// <summary>
        /// Gets the <typeparamref name="T">instance</typeparamref> of a service.
        /// </summary>
        /// <param name="context">The <see cref="ExecutionContext"/> to use while resolving the dependency.</param>
        /// <typeparam name="T">The <see cref="Type" /> of the service to get.</typeparam>
        /// <returns>The instance.</returns>
        T Get<T>(ExecutionContext context)
            where T : class;

        /// <summary>
        /// Gets the <typeparamref name="T">instance</typeparamref> of a service.
        /// </summary>
        /// <param name="tenant">The <see cref="TenantId"/> to use while resolving the dependency.</param>
        /// <typeparam name="T">The <see cref="Type" /> of the service to get.</typeparam>
        /// <returns>The instance.</returns>
        T Get<T>(TenantId tenant)
            where T : class;

        /// <summary>
        /// Gets the <see cref="IServiceProvider"/> for a specific <see cref="ExecutionContext"/>.
        /// </summary>
        /// <param name="executionContext">The <see cref="ExecutionContext"/>.</param>
        /// <returns>The <see cref="IServiceProvider"/>.</returns>
        IServiceProvider GetProviderFor(ExecutionContext executionContext);

        /// <summary>
        /// Gets the <see cref="IServiceProvider"/> for a specific <see cref="TenantId"/>.
        /// </summary>
        /// <param name="tenant">The <see cref="TenantId"/>.</param>
        /// <returns>The <see cref="IServiceProvider"/>.</returns>
        IServiceProvider GetProviderFor(TenantId tenant);
    }
}