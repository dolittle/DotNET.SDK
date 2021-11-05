// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using BaselineTypeDiscovery;

namespace Dolittle.SDK
{
    /// <summary>
    /// Exception that gets thrown when the <see cref="AssemblyFinder"/> fails to load the assembly from a file.
    /// </summary>
    public class CouldNotLoadAssemblyFromFile : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CouldNotLoadAssemblyFromFile"/> class.
        /// </summary>
        /// <param name="failedFile">The file that failed to load.</param>
        public CouldNotLoadAssemblyFromFile(string failedFile)
            : base($"Could not load assembly from file {failedFile}")
        {
        }
    }
}