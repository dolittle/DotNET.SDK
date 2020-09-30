// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.DependencyInversion
{
    /// <summary>
    /// Exception that gets thrown when the default container implementation gets asked for an instance that
    /// has  does not have a default constructor.
    /// </summary>
    public class DefaultContainerDoesNotSupportConstructorArguments : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultContainerDoesNotSupportConstructorArguments"/> class.
        /// </summary>
        /// <param name="service">The <see cref="Type" /> of the service that does not have a default constructor.</param>
        public DefaultContainerDoesNotSupportConstructorArguments(Type service)
            : base($"The default container is not capable of creating instances of {service}, because it has no constructor without arguments")
        {
        }
    }
}