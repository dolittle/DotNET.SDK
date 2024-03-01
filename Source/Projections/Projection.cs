// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Diagnostics;
using Dolittle.SDK.Events;
using Dolittle.SDK.Execution;
using Dolittle.SDK.Projections.Builder;

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
        ProjectionModelId identifier,
        IDictionary<EventType, IProjectionMethod<TReadModel>> onMethods)
    {
        _onMethods = onMethods;
        Identifier = identifier.Id;
        ScopeId = identifier.Scope;
        Events = onMethods.Select(_ => new EventSelector(_.Key, _.Value.KeySelector)).ToList();
        ProjectionType = typeof(TReadModel);

        if (!string.IsNullOrEmpty(identifier.Alias))
        {
            Alias = identifier.Alias;
        }
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
    public ProjectionAlias? Alias { get; }

    /// <inheritdoc />
    public bool HasAlias => Alias is not null;

    /// <inheritdoc/>
    public Task<ProjectionResult<TReadModel>> On(TReadModel readModel, object @event, EventType eventType, ProjectionContext context,
        CancellationToken cancellation)
    {
        using var activity = context.EventContext.CommittedExecutionContext.StartChildActivity("Projection on " + @event.GetType().Name)?.Tag(eventType);

        try
        {
            if (!_onMethods.TryGetValue(eventType, out var method))
            {
                throw new MissingOnMethodForEventType(eventType);
            }

            return Task.FromResult(method.TryOn(readModel, @event, context));
        }
        catch (Exception e)
        {
            activity?.RecordError(e);
            throw new ProjectionOnMethodFailed(Identifier, eventType, @event, e);
            
        }
    }
}
