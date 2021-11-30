// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK
{
    /// <summary>
    /// Exception that gets thrown when trying to resolve <see cref="IDolittleClient"/> from the <see cref="IServiceProvider"/> and it is not registered.
    /// </summary>
    public class DolittleClientNotSetup : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DolittleClientNotSetup"/> class.
        /// </summary>
        public DolittleClientNotSetup()
            : base("The Dolittle Client has not been set up. You need to call UseDolittle() on the Host Builder or AddDolittle() on the Service Collection")
        {
        }
    }
}