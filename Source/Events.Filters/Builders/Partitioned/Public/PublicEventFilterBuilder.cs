// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.ApplicationModel.ClientSetup;
using Dolittle.SDK.ApplicationModel;

namespace Dolittle.SDK.Events.Filters.Builders.Partitioned.Public;

/// <summary>
/// Represents the builder for building public event filters.
/// </summary>
public class PublicEventFilterBuilder : IPartitionedEventFilterBuilder, ICanTryBuildFilter
{
    readonly FilterModelId _filterId;
    PartitionedFilterEventCallback? _callback;

    /// <summary>
    /// Initializes a new instance of the <see cref="PublicEventFilterBuilder"/> class.
    /// </summary>
    /// <param name="filterId">The <see cref="_filterId" />.</param>
    /// <param name="modelBuilder">The <see cref="IClientBuildResults"/>.</param>
    public PublicEventFilterBuilder(FilterId filterId, IModelBuilder modelBuilder)
    {
        _filterId = new FilterModelId(filterId, ScopeId.Default, "");
        modelBuilder.BindIdentifierToProcessorBuilder<ICanTryBuildFilter, FilterModelId, FilterId>(_filterId, this);
    }
    
    /// <inheritdoc />
    public bool Equals(IProcessorBuilder<FilterModelId, FilterId> other) => other is PublicEventFilterBuilder && ReferenceEquals(this, other);
    
    /// <inheritdoc />
    public void Handle(PartitionedFilterEventCallback callback)
        => _callback = callback;


    /// <inheritdoc />
    public bool TryBuild(FilterModelId filterModelId, IClientBuildResults buildResults, out ICanRegisterEventFilterProcessor filter)
    {
        filter = default;
        if (_callback == default)
        {
            buildResults.AddError(new MissingFilterCallback(_filterId));
            return false;
        }
        filter = new UnregisteredPublicEventFilter(_filterId, _callback);
        return true;
    }

}
