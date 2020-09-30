// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.DependencyInversion
{
    /// <summary>
    /// Defines an IoC container for dependency inversion.
    /// </summary>
    public interface IContainer
    {
        /// <summary>
        /// Gets the instance of a service.
        /// </summary>
        /// <param name="service">The <see cref="Type" /> of service to get.</param>
        /// <returns>The instance.</returns>
        object Get(Type service);

        /// <summary>
        /// Gets the <typeparamref name="T">instance</typeparamref> of a service.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type" /> of the service to get.</typeparam>
        /// <returns>The instance.</returns>
        T GetFor<T>()
            where T : class;
    }
}