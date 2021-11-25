// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.DependencyInversion
{
    /// <summary>
    /// Represents a wrapper for an <see cref="IServiceProvider"/>. 
    /// </summary>
    public class RootProviderWrapper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RootProviderWrapper"/> class.
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/>.</param>
        public RootProviderWrapper(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            Console.WriteLine("Creating new wrapper");
        }

        /// <summary>
        /// Gets the <see cref="IServiceProvider"/>.
        /// </summary>
        public IServiceProvider ServiceProvider { get; }
    }
}