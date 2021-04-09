// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Services.Contracts;
using ExecutionContext = Dolittle.SDK.Execution.ExecutionContext;

namespace Dolittle.SDK.Services
{
    /// <summary>
    /// Defines a system that can resolve <see cref="CallRequestContext" />.
    /// </summary>
    public interface IResolveCallContext
    {
        /// <summary>
        /// Resolves the current <see cref="CallRequestContext" /> from the <see cref="ExecutionContext" />.
        /// </summary>
        /// <param name="executionContext">The <see cref="ExecutionContext" /> to resolve from.</param>
        /// <returns>The resolved <see cref="CallRequestContext" />.</returns>
        CallRequestContext ResolveFrom(ExecutionContext executionContext);
    }
}
