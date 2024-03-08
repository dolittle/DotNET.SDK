// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.SDK.Projections.Builder;

public static class ProjectionSignatureMapper
{
    public static ProjectionSignature<TProjection, TEvent> Map<TProjection, TEvent>(ProjectionEventContextSignature<TProjection, TEvent> signature)
        where TProjection : ReadModel, new()
        where TEvent : class =>
        (instance, @event, context) => signature(instance, @event, context.EventContext);

    public static ProjectionSignature<TProjection, TEvent> Map<TProjection, TEvent>(ProjectionEventSignature<TProjection, TEvent> signature)
        where TProjection : ReadModel, new()
        where TEvent : class =>
        (instance, @event, _) => signature(instance, @event);

    public static ProjectionSignature<TProjection, TEvent> Map<TProjection, TEvent>(ProjectionMethodSignature<TProjection, TEvent> signature)
        where TProjection : ReadModel, new()
        where TEvent : class =>
        (instance, @event, context) =>
        {
            signature(instance, @event, context);
            return ProjectionResult<TProjection>.Replace(instance);
        };

    public static ProjectionSignature<TProjection, TEvent> Map<TProjection, TEvent>(ProjectionMethodEventContextSignature<TProjection, TEvent> signature)
        where TProjection : ReadModel, new()
        where TEvent : class =>
        (instance, @event, context) =>
        {
            signature(instance, @event, context.EventContext);
            return ProjectionResult<TProjection>.Replace(instance);
        };

    public static ProjectionSignature<TProjection, TEvent> Map<TProjection, TEvent>(ProjectionMethodEventSignature<TProjection, TEvent> signature)
        where TProjection : ReadModel, new()
        where TEvent : class =>
        (instance, @event, _) =>
        {
            signature(instance, @event);
            return ProjectionResult<TProjection>.Replace(instance);
        };

    public static ProjectionSignature<TProjection, TEvent> Map<TProjection, TEvent>(ProjectionResultTypeSignature<TProjection, TEvent> signature)
        where TProjection : ReadModel, new()
        where TEvent : class =>
        (instance, @event, context) =>
        {
            var projectionResultType = signature(instance, @event, context);
            return ProjectionResult<TProjection>.ToResult(instance, projectionResultType);
        };

    public static ProjectionSignature<TProjection, TEvent> Map<TProjection, TEvent>(ProjectionResultTypeEventContextSignature<TProjection, TEvent> signature)
        where TProjection : ReadModel, new()
        where TEvent : class =>
        (instance, @event, context) =>
        {
            var projectionResultType = signature(instance, @event, context.EventContext);
            return ProjectionResult<TProjection>.ToResult(instance, projectionResultType);
        };

    public static ProjectionSignature<TProjection, TEvent> Map<TProjection, TEvent>(ProjectionResultTypeEventSignature<TProjection, TEvent> signature)
        where TProjection : ReadModel, new()
        where TEvent : class =>
        (instance, @event, _) =>
        {
            var projectionResultType = signature(instance, @event);
            return ProjectionResult<TProjection>.ToResult(instance, projectionResultType);
        };

    public static ProjectionSignature<TProjection> Map<TProjection>(ProjectionResultTypeEventSignature<TProjection> signature)
        where TProjection : ReadModel, new() =>
        (instance, @event, _) =>
        {
            var projectionResultType = signature(instance, @event);
            return ProjectionResult<TProjection>.ToResult(instance, projectionResultType);
        };
    
    public static ProjectionSignature<TProjection> Map<TProjection>(ProjectionResultTypeEventContextSignature<TProjection> signature)
        where TProjection : ReadModel, new() =>
        (instance, @event, context) =>
        {
            var projectionResultType = signature(instance, @event, context.EventContext);
            return ProjectionResult<TProjection>.ToResult(instance, projectionResultType);
        };
    
    public static ProjectionSignature<TProjection> Map<TProjection>(ProjectionResultTypeSignature<TProjection> signature)
        where TProjection : ReadModel, new() =>
        (instance, @event, context) =>
        {
            var projectionResultType = signature(instance, @event, context);
            return ProjectionResult<TProjection>.ToResult(instance, projectionResultType);
        };
    
    public static ProjectionSignature<TProjection> Map<TProjection>(ProjectionMethodSignature<TProjection> signature)
        where TProjection : ReadModel, new() =>
        (instance, @event, context) =>
        {
            signature(instance, @event, context);
            return ProjectionResult<TProjection>.Replace(instance);
        };
    
    public static ProjectionSignature<TProjection> Map<TProjection>(ProjectionMethodEventContextSignature<TProjection> signature)
        where TProjection : ReadModel, new() =>
        (instance, @event, context) =>
        {
            signature(instance, @event, context.EventContext);
            return ProjectionResult<TProjection>.Replace(instance);
        };
    
    public static ProjectionSignature<TProjection> Map<TProjection>(ProjectionMethodEventSignature<TProjection> signature)
        where TProjection : ReadModel, new() =>
        (instance, @event, _) =>
        {
            signature(instance, @event);
            return ProjectionResult<TProjection>.Replace(instance);
        };
    
    public static ProjectionSignature<TProjection> Map<TProjection>(ProjectionEventSignature<TProjection> signature)
        where TProjection : ReadModel, new() =>
        (instance, @event, _) => signature(instance, @event);
    
    public static ProjectionSignature<TProjection> Map<TProjection>(ProjectionEventContextSignature<TProjection> signature)
        where TProjection : ReadModel, new() =>
        (instance, @event, context) => signature(instance, @event, context.EventContext);

}
