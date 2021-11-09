// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using BaselineTypeDiscovery;

namespace Dolittle.SDK
{
    /// <summary>
    /// Extensions for the <see cref="DolittleClientBuilder"/> to register artifacts by loading all assemblies in the current directory.
    /// </summary>
    public static class ClientBuilderExtensions
    {
        /// <summary>
        /// Registers all event types by loading all assemblies in the current directory.
        /// </summary>
        /// <param name="builder">The <see cref="DolittleClientBuilder"/> to use.</param>
        /// <returns>The client builder for continuation.</returns>
        public static DolittleClientBuilder WithAllEventTypes(this DolittleClientBuilder builder)
            => ForAllAllScannedAssemblies(builder, (clientBuilder, assembly) => clientBuilder.WithEventTypes(_ => _.RegisterAllFrom(assembly)));

        /// <summary>
        /// Registers all aggregate roots by loading all assemblies in the current directory.
        /// </summary>
        /// <param name="builder">The <see cref="DolittleClientBuilder"/> to use.</param>
        /// <returns>The client builder for continuation.</returns>
        public static DolittleClientBuilder WithAllAggregateRoots(this DolittleClientBuilder builder)
            => ForAllAllScannedAssemblies(builder, (clientBuilder, assembly) => clientBuilder.WithAggregateRoots(_ => _.RegisterAllFrom(assembly)));

        /// <summary>
        /// Registers all event handlers by loading all assemblies in the current directory.
        /// </summary>
        /// <param name="builder">The <see cref="DolittleClientBuilder"/> to use.</param>
        /// <returns>The client builder for continuation.</returns>
        public static DolittleClientBuilder WithAllEventHandlers(this DolittleClientBuilder builder)
            => ForAllAllScannedAssemblies(builder, (clientBuilder, assembly) => clientBuilder.WithEventHandlers(_ => _.RegisterAllFrom(assembly)));

        /// <summary>
        /// Registers all projections by loading all assemblies in the current directory.
        /// </summary>
        /// <param name="builder">The <see cref="DolittleClientBuilder"/> to use.</param>
        /// <returns>The client builder for continuation.</returns>
        public static DolittleClientBuilder WithAllProjections(this DolittleClientBuilder builder)
            => ForAllAllScannedAssemblies(builder, (clientBuilder, assembly) => clientBuilder.WithProjections(_ => _.RegisterAllFrom(assembly)));

        /// <summary>
        /// Registers all embeddings by loading all assemblies in the current directory.
        /// </summary>
        /// <param name="builder">The <see cref="DolittleClientBuilder"/> to use.</param>
        /// <returns>The client builder for continuation.</returns>
        public static DolittleClientBuilder WithAllEmbeddings(this DolittleClientBuilder builder)
            => ForAllAllScannedAssemblies(builder, (clientBuilder, assembly) => clientBuilder.WithEmbeddings(_ => _.RegisterAllFrom(assembly)));

        static DolittleClientBuilder ForAllAllScannedAssemblies(DolittleClientBuilder builder, Action<DolittleClientBuilder, Assembly> perform)
        {
            foreach (var assembly in GetAllAssemblies())
            {
                perform(builder, assembly);
            }

            return builder;
        }

        static IEnumerable<Assembly> GetAllAssemblies()
        {
            return AssemblyFinder.FindAssemblies(
                failedFile => throw new CouldNotLoadAssemblyFromFile(failedFile),
                _ => true,
                false);
        }
    }
}