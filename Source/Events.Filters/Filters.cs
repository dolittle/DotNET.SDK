// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Events.Filters
{
    /// <summary>
    /// Represents an implementation of <see cref="IFilters" />.
    /// </summary>
    public class Filters : IFilters
    {
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Filters"/> class.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public Filters(ILogger logger) => _logger = logger;

        /// <inheritdoc/>
        public void Register(IFilterProcessor filter, CancellationToken cancellation = default)
            => filter.RegisterForeverWithPolicy(new Processing.Internal.RetryPolicy(), cancellation)
                .Subscribe(
                    _ => { },
                    error => _logger.LogError("Failed to register filter {Error}", error),
                    () => _logger.LogError("Filter registration completed"));
    }
}
