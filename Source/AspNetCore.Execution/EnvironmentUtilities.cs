// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Execution;

namespace Dolittle.AspNetCore.Execution
{
    /// <summary>
    /// Helper methods for <see cref="System.Environment"/>.
    /// </summary>
    public static class EnvironmentUtilities
    {
        const string EnvironmentVariable = "ASPNETCORE_ENVIRONMENT";

        /// <summary>
        /// Get <see cref="Environment">execution environment</see>.
        /// </summary>
        /// <returns><see cref="Environment">execution environment</see>.</returns>
        public static Environment GetExecutionEnvironment()
        {
            switch (System.Environment.GetEnvironmentVariable(EnvironmentVariable)?.ToUpperInvariant() ?? "undetermined")
            {
                case "DEVELOPMENT":
                    return Environment.Development;
                case "PRODUCTION":
                    return Environment.Production;
            }

            return Environment.Undetermined;
        }
    }
}
