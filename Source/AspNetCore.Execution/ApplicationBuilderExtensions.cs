// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.AspNetCore.Execution;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// Extensions for <see cref="IApplicationBuilder"/>.
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Use Dolittle for the given application.
        /// </summary>
        /// <param name="app"><see cref="IApplicationBuilder"/> to use Dolittle for.</param>
        public static void UseDolittleExecutionContext(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExecutionContextSetup>();
        }
    }
}