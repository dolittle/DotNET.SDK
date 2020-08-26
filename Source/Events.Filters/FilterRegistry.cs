// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Events.Filters.EventHorizon;
using Dolittle.Events.Filters.Internal;
using Dolittle.Resilience;

namespace Dolittle.Events.Filters
{
    /// <summary>
    /// An implementation of <see cref="IRegisterFilters"/>.
    /// </summary>
    public class FilterRegistry : IRegisterFilters
    {
        readonly FilterProcessors _processors;
        readonly IAsyncPolicyFor<FilterRegistry> _policy;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterRegistry"/> class.
        /// </summary>
        /// <param name="processors">The <see cref="FilterProcessors"/> to use for creating filter processors.</param>
        /// <param name="policy">The <see cref="IAsyncPolicyFor{T}"/> that defines reconnect policies for the event filters.</param>
        public FilterRegistry(
            FilterProcessors processors,
            IAsyncPolicyFor<FilterRegistry> policy)
        {
            _processors = processors;
            _policy = policy;
        }

        /// <inheritdoc/>
        public Task Register(FilterId id, ScopeId scope, ICanFilterEvents filter, CancellationToken cancellationToken)
            => _processors.GetFor(id, scope, filter).RegisterAndHandleForeverWithPolicy(_policy, cancellationToken);

        /// <inheritdoc/>
        public Task Register(FilterId id, ScopeId scope, ICanFilterEventsWithPartition filter, CancellationToken cancellationToken)
            => _processors.GetFor(id, scope, filter).RegisterAndHandleForeverWithPolicy(_policy, cancellationToken);

        /// <inheritdoc/>
        public Task Register(FilterId id, ICanFilterPublicEvents filter, CancellationToken cancellationToken)
            => _processors.GetFor(id, filter).RegisterAndHandleForeverWithPolicy(_policy, cancellationToken);
    }
}