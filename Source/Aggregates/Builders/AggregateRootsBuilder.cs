// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dolittle.SDK.DependencyInversion;

namespace Dolittle.SDK.Aggregates.Builders;

/// <summary>
/// Represents an implementation of <see cref="IAggregateRootsBuilder"/>.
/// </summary>
public class AggregateRootsBuilder : IAggregateRootsBuilder
{
    readonly Dictionary<Type, AggregateRootType> _associations = new();
    
    /// <inheritdoc />
    public IAggregateRootsBuilder Register<T>()
        where T : class
        => Register(typeof(T));
    
    /// <inheritdoc />
    public IAggregateRootsBuilder Register(Type type)
    {
        ThrowIfTypeIsMissingAggregateRootAttribute(type);
        TryGetAggregateRootTypeFromAttribute(type, out var eventType);
        AddAssociation(type, eventType);
        return this;
    }
    
    /// <inheritdoc />
    public IAggregateRootsBuilder RegisterAllFrom(Assembly assembly)
    {
        foreach (var type in assembly.ExportedTypes.Where(IsAggregateRoot))
        {
            Register(type);
        }

        return this;
    }

    /// <summary>
    /// Builds the aggregate roots by registering them with the Runtime.
    /// </summary>
    /// <param name="aggregateRoots">The <see cref="Internal.AggregateRootsClient"/>.</param>
    /// <param name="tenantScopedProvidersBuilder">The <see cref="TenantScopedProvidersBuilder"/>.</param>
    /// <param name="aggregatesBuilder">The <see cref="IAggregatesBuilder"/>.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public IUnregisteredAggregateRoots Build()
    {
        var result = new UnregisteredAggregateRoots();
        foreach (var (type, aggregateRootType) in _associations)
        {
            result.Associate(type, aggregateRootType);
        }
        return result;
    }

    static bool IsAggregateRoot(Type type)
        => type.GetCustomAttributes(typeof(AggregateRootAttribute), true).FirstOrDefault() is AggregateRootAttribute;

    static bool TryGetAggregateRootTypeFromAttribute(Type type, out AggregateRootType aggregateRootType)
    {
        if (Attribute.GetCustomAttribute(type, typeof(AggregateRootAttribute)) is AggregateRootAttribute attribute)
        {
            if (!attribute.Type.HasAlias)
            {
                aggregateRootType = new AggregateRootType(attribute.Type.Id, attribute.Type.Generation, type.Name);
                return true;
            }

            aggregateRootType = attribute.Type;
            return true;
        }

        aggregateRootType = default;
        return false;
    }

    void AddAssociation(Type type, AggregateRootType aggregateRootType)
    {
        _associations[type] = aggregateRootType;
    }

    void ThrowIfTypeIsMissingAggregateRootAttribute(Type type)
    {
        if (!TryGetAggregateRootTypeFromAttribute(type, out _))
        {
            throw new TypeIsMissingAggregateRootAttribute(type);
        }
    }
}
