// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.SDK.Execution;

namespace Dolittle.SDK
{
    /// <summary>
    /// Defines a system that can resolve the <see cref="ExecutionContext"/> for the <see cref="IDolittleClient"/>.
    /// </summary>
    public interface ICanResolveExecutionContextForDolittleClient
    {
        /// <summary>
        /// Resolves the <see cref="ExecutionContext"/> used by the <see cref="IDolittleClient"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> that, when resolved, returns the <see cref="ExecutionContext"/>.</returns>
        public Task<ExecutionContext> Resolve();
    }
}