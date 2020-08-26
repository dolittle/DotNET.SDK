// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Microservice.Configuration
{
    /// <summary>
    /// Exception that gets thrown when there is no configuration for the microservice.
    /// </summary>
    public class MissingMicroserviceConfiguration : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingMicroserviceConfiguration"/> class.
        /// </summary>
        /// <param name="path">Search path.</param>
        public MissingMicroserviceConfiguration(string path)
            : base($"Missing configuration for the microservice - looking for file 'microservice.json' at path {path}")
        {
        }
    }
}