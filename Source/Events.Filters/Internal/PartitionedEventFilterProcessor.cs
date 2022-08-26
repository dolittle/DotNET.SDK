// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.SDK.Events.Processing;
using Dolittle.SDK.Protobuf;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Events.Filters.Internal;

/// <summary>
/// Represents a <see cref="FilterEventProcessor{TRegisterArguments, TResponse}" /> that can filter partitioned private events.
/// </summary>
public class PartitionedEventFilterProcessor : FilterEventProcessor<PartitionedFilterRegistrationRequest, PartitionedFilterResponse>
{
    readonly PartitionedFilterEventCallback _filterEventCallback;
    readonly FilterModelId _filterId;

    /// <summary>
    /// Initializes a new instance of the <see cref="PartitionedEventFilterProcessor"/> class.
    /// </summary>
    /// <param name="filterId">The <see cref="FilterModelId" />.</param>
    /// <param name="filterEventCallback">The <see cref="PartitionedFilterEventCallback" />.</param>
    /// <param name="converter">The <see cref="IEventProcessingConverter" />.</param>
    /// <param name="loggerFactory">The <see cref="ILoggerFactory" />.</param>
    public PartitionedEventFilterProcessor(
        FilterModelId filterId,
        PartitionedFilterEventCallback filterEventCallback,
        IEventProcessingConverter converter,
        ILoggerFactory loggerFactory)
        : base("Partitioned Filter", filterId.Id, converter, loggerFactory)
    {
        _filterId = filterId;
        _filterEventCallback = filterEventCallback;
    }

    /// <inheritdoc/>
    public override PartitionedFilterRegistrationRequest RegistrationRequest
        => new()
        {
            FilterId = _filterId.Id.ToProtobuf(),
            ScopeId = _filterId.Scope.ToProtobuf(),
        };

    /// <inheritdoc/>
    protected override PartitionedFilterResponse CreateResponseFromFailure(ProcessorFailure failure)
        => new() { Failure = failure };

    /// <inheritdoc/>
    protected override async Task<PartitionedFilterResponse> Filter(object @event, EventContext context, CancellationToken cancellation)
    {
        var result = await _filterEventCallback(@event, context).ConfigureAwait(false);
        return new PartitionedFilterResponse { IsIncluded = result.ShouldInclude, PartitionId = result.PartitionId.Value };
    }
}
