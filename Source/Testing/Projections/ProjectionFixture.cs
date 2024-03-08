// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Common.Model;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Builders;
using Dolittle.SDK.Projections;
using Dolittle.SDK.Projections.Builder;
using Dolittle.SDK.Projections.Core;
using Dolittle.SDK.Projections.Internal;

namespace Dolittle.SDK.Testing.Projections;

public static class ProjectionFixture<TProjection>
    where TProjection : ReadModel, new()
{
    static Exception? Error { get; } = default;
    static readonly IProjection<TProjection>? _projection;
    public static IProjection<TProjection> Projection => _projection ?? throw new TypeInitializationException("Failed to create projection", Error);

    static ProjectionFixture()
    {
        var id = ProjectionType<TProjection>.ProjectionModelId;
        if (id is null)
        {
            Error = new MissingProjectionAttribute(typeof(TProjection));
            _projection = null;
            return;
        }

        var clientBuildResults = new ClientBuildResults();
        var modelBuilder = new ModelBuilder();
        var eventTypesBuilder = new EventTypesBuilder(modelBuilder, clientBuildResults);
        foreach (var handledEventType in ProjectionType<TProjection>.HandledEventTypes)
        {
            eventTypesBuilder.Register(handledEventType);
        }

        var build = modelBuilder.Build(clientBuildResults);
        IEventTypes eventTypes = EventTypesBuilder.Build(build, clientBuildResults);

        if (clientBuildResults.Failed || !new ConventionProjectionBuilder<TProjection>(id)
            .TryBuild(id, eventTypes, clientBuildResults, out var projection))
        {
            Error = new ArgumentException(clientBuildResults.ErrorString);
            _projection = null;
            return;
        }

        _projection = (IProjection<TProjection>?)projection;
    }
}
