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
public static class ProjectionType<TProjection> where TProjection : ReadModel, new()
{
    // ReSharper disable StaticMemberInGenericType
    public static HashSet<Type> HandledEventTypes { get; }
    public static ProjectionModelId? ProjectionModelId { get; }
    
    public static TimeSpan? IdleUnloadTimeout { get; }

    static ProjectionType()
    {
        HandledEventTypes = HandledEvents().ToHashSet();
        var attributeMetadata = GetAttributeMetadata();
        if (attributeMetadata.HasValue)
        {
            ProjectionModelId = attributeMetadata.Value.id;
            IdleUnloadTimeout = attributeMetadata.Value.timeout;
        }
        else
        {
            ProjectionModelId = null;
            IdleUnloadTimeout = null;
        }
        
    }

    static IEnumerable<Type> HandledEvents()
    {
        var projectionType = typeof(TProjection);
        var methods = projectionType
            .GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic)
            // "On" methods only
            .Where(method => method.Name == "On");
        foreach (var method in methods)
        {
            if (method.GetParameters().Length is 1 or 2 && method.GetParameters()[0].ParameterType.GetCustomAttribute<EventTypeAttribute>() != null)
            {
                yield return method.GetParameters()[0].ParameterType;
            }
        }
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
    static (ProjectionModelId id, TimeSpan? timeout)? GetAttributeMetadata()
    {
        var aggregateRootType = typeof(TProjection);
        var aggregateRootAttribute = aggregateRootType.GetCustomAttribute<ProjectionAttribute>();
        if (aggregateRootAttribute is null)
        {
            return null;
        }
        
        
        return (aggregateRootAttribute.GetIdentifier(aggregateRootType), aggregateRootAttribute.IdleUnloadTimeout);
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
