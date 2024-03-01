// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dolittle.Runtime.Aggregates.Management.Contracts;
using Dolittle.SDK.Events;
using Dolittle.SDK.Projections.Core;

namespace Dolittle.SDK.Projections.Internal;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TProjection"></typeparam>
public static class ProjectionType<TProjection> where TProjection : ProjectionBase, new()
{
    public delegate ProjectionResult<TProjection> ApplyMethod(TProjection projection, object @event, ProjectionContext context);

    // ReSharper disable StaticMemberInGenericType
    public static IReadOnlyDictionary<Type, ApplyMethod> MethodsPerEventType { get; }
    public static ProjectionModelId? ProjectionModelId { get; }

    static ProjectionType()
    {
        MethodsPerEventType = CreateMethodsPerEventType();
        ProjectionModelId = GetProjectionModelId();
    }

    /// <summary>
    /// Creates a new instance of the <see cref="TProjection" />.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static TProjection Create(Key key)
    {
        return new TProjection
        {
            Id = key.Value
        };
    }

    /// <summary>
    /// Gets the <see cref="AggregateRootId" /> of an <see cref="AggregateRoot" />.
    /// </summary>
    /// <returns>The <see cref="AggregateRootId" />.</returns>
    public static ProjectionModelId GetProjectionId()
    {
        if (ProjectionModelId == null)
        {
            throw new MissingProjectionAttribute(typeof(TProjection));
        }

        return ProjectionModelId;
    }

    /// <summary>
    /// Gets the <see cref="AggregateRootId" /> of an <see cref="AggregateRoot" />.
    /// </summary>
    /// <returns>The <see cref="AggregateRootId" />.</returns>
    static ProjectionModelId? GetProjectionModelId()
    {
        var aggregateRootType = typeof(TProjection);
        var aggregateRootAttribute = aggregateRootType.GetCustomAttribute<ProjectionAttribute>();
        return aggregateRootAttribute?.GetIdentifier(aggregateRootType);
    }

    static IReadOnlyDictionary<Type, ApplyMethod> CreateMethodsPerEventType()
    {
        var projectionType = typeof(TProjection);
        var applyMethods = new Dictionary<Type, ApplyMethod>();
        var keySelectors = new Dictionary<Type, KeySelector>();
        foreach (var method in projectionType
            .GetTypeInfo()
            .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(m
                => m.Name.Equals("On", StringComparison.InvariantCultureIgnoreCase)
                   && ParametersAreValid(m.GetParameters())
            )
        )
        {
            if (applyMethods.TryAdd(method.GetParameters()[0].ParameterType, CreateApplyMethod(method)))
            {
                applyMethods[method.GetParameters()[0].ParameterType] = CreateApplyMethod(method);
            }
            else
            {
                throw new DuplicateHandlerForEventType(method.GetParameters()[0].ParameterType, typeof(TProjection));
            }
        }

        return applyMethods;
    }

    static bool ParametersAreValid(ParameterInfo[] parameters)
    {
        if (parameters.Length is 0 or > 2)
        {
            return false;
        }

        // First parameter must be an event
        var firstParameter = parameters[0];
        if (!firstParameter.ParameterType.IsClass || firstParameter.ParameterType.GetCustomAttribute<EventTypeAttribute>() == null)
        {
            return false;
        }

        // Optionally just the event
        if (parameters.Length == 1)
        {
            return true;
        }

        // Or the event and the context. The context can be either EventContext or ProjectionContext
        var secondParameter = parameters[1];
        return secondParameter.ParameterType == typeof(EventContext) || secondParameter.ParameterType == typeof(ProjectionContext);
    }

    public static ProjectionResult<TProjection> Apply(TProjection projection, object someEvent, ProjectionContext projectionContext)
    {
        var type = someEvent.GetType();
        if (!MethodsPerEventType.TryGetValue(type, out var method))
        {
            throw new MissingOnMethodForEventType(type, typeof(TProjection));
        }

        return method(projection, someEvent, projectionContext);
    }

    static ApplyMethod CreateApplyMethod(MethodInfo method)
    {
        switch (method.ReturnType)
        {
            // void return
            case { } returnType when returnType == typeof(void):
                return CreateForVoidReturn(method);
            // ProjectionResultType
            case { } returnType when returnType == typeof(ProjectionResult<TProjection>):
                return CreateForProjectionResultReturn(method);
            default:
                throw new UnsupportedReturnType(method);
        }
    }

    static ApplyMethod CreateForProjectionResultReturn(MethodInfo method)
    {
        if (method.GetParameters().Length == 1)
        {
            return (projection, @event, _) => ToResult(method.Invoke(projection, new[] { @event }));
        }

        if (method.GetParameters()[1].ParameterType == typeof(EventContext))
        {
            return (projection, @event, context) =>
            {
                // Unpack the projection context
                return ToResult(method.Invoke(projection, new[] { @event, context.EventContext }));
            };
        }


        return (projection, @event, context) => { return ToResult(method.Invoke(projection, new[] { @event, context })); };

        static ProjectionResult<TProjection> ToResult(object? result)
        {
            if (result is null)
            {
                return ProjectionResult<TProjection>.Delete;
            }

            return (ProjectionResult<TProjection>)result;
        }
    }

    static ApplyMethod CreateForVoidReturn(MethodInfo method)
    {
        if (method.GetParameters().Length == 1)
        {
            return (projection, @event, _) =>
            {
                method.Invoke(projection, new[] { @event });
                return projection;
            };
        }

        if (method.GetParameters()[1].ParameterType == typeof(EventContext))
        {
            return (projection, @event, context) =>
            {
                // Unpack the projection context
                method.Invoke(projection, new[] { @event, context.EventContext });
                return projection;
            };
        }


        return (projection, @event, context) =>
        {
            method.Invoke(projection, new[] { @event, context });
            return projection;
        };
    }
}

class DuplicateHandlerForEventType : Exception
{
    public DuplicateHandlerForEventType(Type parameterType, Type type) : base(
        $"Duplicate handler for event type '{parameterType.FullName}' in projection '{type.FullName}'")
    {
    }
}

class UnsupportedReturnType : Exception
{
    public UnsupportedReturnType(MethodInfo method) : base(
        $"Unsupported return type '{method.ReturnType}' for method '{method.Name}' in projection '{method.DeclaringType?.FullName}'")
    {
    }
}
