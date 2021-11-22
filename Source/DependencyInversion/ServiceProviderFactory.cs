// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.SDK.DependencyInversion
{
    /// <summary>
    /// Represents an implementation of <see cref="IServiceProviderFactory{TContainerBuilder}"/> for <see cref="ContainerBuilder"/>.
    /// </summary>
    public class ServiceProviderFactory : IServiceProviderFactory<ContainerBuilder>
    {
        readonly Action<ContainerBuilder> _configure;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceProviderFactory"/> class.
        /// </summary>
        /// <param name="configure">The optional <see cref="Action{T}"/> for configuring <see cref="ContainerBuilder"/>.</param>
        public ServiceProviderFactory(Action<ContainerBuilder> configure = null)
        {
            _configure = configure;
        }

        /// <inheritdoc />
        public ContainerBuilder CreateBuilder(IServiceCollection services)
        {
            var builder = new ContainerBuilder();
            builder.PopulateRootContainer(services);
            _configure?.Invoke(builder);
            return builder;
        }

        /// <inheritdoc />
        public IServiceProvider CreateServiceProvider(ContainerBuilder containerBuilder)
            => containerBuilder.Build();
    }
}