// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Execution;

namespace Dolittle.SDK.DependencyInversion
{
    /// <summary>
    /// Represents an implementation of <see cref="IContainer" />.
    /// </summary>
    public class DefaultContainer : IContainer
    {
        /// <inheritdoc/>
        public object Get(Type service, ExecutionContext context)
        {
            ThrowIfMissingDefaultConstructor(service);
            return Activator.CreateInstance(service);
        }

        /// <inheritdoc/>
        public T Get<T>(ExecutionContext context)
            where T : class
            => Get(typeof(T), context) as T;

        void ThrowIfMissingDefaultConstructor(Type service)
        {
            if (service.GetConstructor(Type.EmptyTypes) == default) throw new DefaultContainerDoesNotSupportConstructorArguments(service);
        }
    }
}
