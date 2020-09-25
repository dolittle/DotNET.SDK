// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.DependencyInversion
{
    /// <summary>
    /// Represents an implementation of <see cref="IContainer" />.
    /// </summary>
    public class Container : IContainer
    {
        /// <inheritdoc/>
        public object Get(Type service)
        {
            ThrowIfMissingDefaultConstructor(service);
            return Activator.CreateInstance(service);
        }

        /// <inheritdoc/>
        public T GetFor<T>()
            where T : class
            => Get(typeof(T)) as T;

        void ThrowIfMissingDefaultConstructor(Type service)
        {
            if (service.GetConstructor(Type.EmptyTypes) == default) throw new DefaultContainerDoesNotSupportConstructorArguments(service);
        }
    }
}