// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Events;
using Dolittle.SDK.Projections.Builder;
using Dolittle.SDK.Projections.Copies;

namespace Dolittle.SDK.Projections;

/// <summary>
/// An implementation of <see cref="IProjection{TReadModel}" />.
/// </summary>
/// <typeparam name="TReadModel">The type of the read model.</typeparam>
public class Projection<TReadModel> : IProjection<TReadModel>
    where TReadModel : class, new()
{
    readonly IDictionary<EventType, IProjectionMethod<TReadModel>> _onMethods;

    /// <summary>
    /// Initializes a new instance of the <see cref="Projection{TReadModel}"/> class.
    /// </summary>
    /// <param name="identifier">The <see cref="ProjectionId" />.</param>
    /// <param name="scopeId">The <see cref="ScopeId" />.</param>
    /// <param name="onMethods">The on methods by <see cref="EventType" />.</param>
    /// <param name="copies">The <see cref="ProjectionCopies"/>.</param>
    public Projection(
        ProjectionId identifier,
        ScopeId scopeId,
        IDictionary<EventType, IProjectionMethod<TReadModel>> onMethods,
        ProjectionCopies copies)
    {
        _onMethods = onMethods;
        Identifier = identifier;
        ScopeId = scopeId;
        Events = onMethods.Select(_ => new EventSelector(_.Key, _.Value.KeySelector)).ToList();
        ProjectionType = typeof(TReadModel);
        Copies = copies;
    }

    /// <inheritdoc />
    public Type ProjectionType { get; }

    /// <inheritdoc/>
    public ProjectionId Identifier { get; }

    /// <inheritdoc/>
    public ScopeId ScopeId { get; }

    /// <inheritdoc/>
    public TReadModel InitialState { get; } = new();

    /// <inheritdoc/>
    public IEnumerable<EventSelector> Events { get; }

    /// <inheritdoc />
    public ProjectionCopies Copies { get; }

    /// <inheritdoc/>
    public async Task<ProjectionResult<TReadModel>> On(TReadModel readModel, object @event, EventType eventType, ProjectionContext context, CancellationToken cancellation)
    {
        if (!_onMethods.TryGetValue(eventType, out var method))
        {
            throw new MissingOnMethodForEventType(eventType);
        }
        var tryOn = await method.TryOn(readModel, @event, context).ConfigureAwait(false);
        if (tryOn.Exception != default)
        {
            throw new ProjectionOnMethodFailed(Identifier, eventType, @event, tryOn.Exception);
        }
        return tryOn.Result;
    }
}
