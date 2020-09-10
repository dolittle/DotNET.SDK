// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.SDK.Execution;
using Dolittle.SDK.Protobuf;
using Dolittle.SDK.Services;
using Microsoft.Extensions.Logging;
using static Dolittle.Runtime.Events.Processing.Contracts.Filters;

namespace Dolittle.SDK.Events.Filters.Internal
{
    /// <summary>
    /// Represents a <see cref="FilterEventProcessor{TRegisterArguments, TResponse}" /> that can filter non-partitioned private events.
    /// </summary>
    public class EventFilterProcessor : FilterEventProcessor<FilterRegistrationRequest, FilterResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventFilterProcessor"/> class.
        /// </summary>
        /// <param name="filterId">The <see cref="FilterId" />.</param>
        /// <param name="eventTypes">The <see cref="EventTypes" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public EventFilterProcessor(
            FilterId filterId,
            ScopeId scopeId,
            Func<object, EventContext, Task<bool>> filterEventCallback,
            FiltersClient client,
            ExecutionContextManager executionContextManager,
            EventTypes eventTypes,
            ILogger<EventFilterProcessor> logger)
            : base("Filter", filterId, eventTypes, logger)
        {

        }

        protected override FilterRegistrationRequest RegisterArguments
            => new FilterRegistrationRequest
                {
                    FilterId = Identifier.ToProtobuf(),
                    ScopeId = ScopeId
                }

        protected override IReverseCallClient<FilterRegistrationRequest, FilterRegistrationResponse, FilterEventRequest, FilterResponse> CreateClient(FilterRegistrationRequest registerArguments, Func<FilterEventRequest, Task<FilterResponse>> callback, uint pingTimeout, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        protected override FilterResponse CreateResponseFromFailure(ProcessorFailure failure)
        {
            throw new NotImplementedException();
        }

        protected override Task<FilterResponse> Filter(object @event, EventContext context)
        {
            throw new NotImplementedException();
        }
    }
}