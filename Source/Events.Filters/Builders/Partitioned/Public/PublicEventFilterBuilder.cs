// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Common.Model;

namespace Dolittle.SDK.Events.Filters.Builders.Partitioned.Public;

/// <summary>
/// Represents the builder for building public event filters.
/// </summary>
public class PublicEventFilterBuilder : IPartitionedEventFilterBuilder, ICanTryBuildFilter
{
    readonly FilterId _filterId;
    PartitionedFilterEventCallback _callback;

    /// <summary>
    /// Initializes a new instance of the <see cref="PublicEventFilterBuilder"/> class.
    /// </summary>
    /// <param name="filterId">The <see cref="_filterId" />.</param>
    /// <param name="modelBuilder">The <see cref="IClientBuildResults"/>.</param>
    public PublicEventFilterBuilder(FilterId filterId, IModelBuilder modelBuilder)
    {
        _filterId = filterId;
        modelBuilder.BindIdentifierToProcessorBuilder<ICanTryBuildFilter>(new FilterModelId(_filterId, ScopeId.Default), this);
    }
    
    /// <inheritdoc />
    public bool Equals(ICanTryBuildFilter other) => ReferenceEquals(this, other);
    
    /// <inheritdoc />
    public void Handle(PartitionedFilterEventCallback callback)
        => _callback = callback;


    /// <inheritdoc />
    public bool TryBuild(IClientBuildResults buildResults, out ICanRegisterEventFilterProcessor filter)
    {
        filter = default;
        if (_callback == default)
        {
            buildResults.AddError(new MissingFilterCallback(_filterId, ScopeId.Default));
            return false;
        }
        filter = new UnregisteredPublicEventFilter(_filterId, _callback);
        return true;
    }

}
