// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Dolittle.SDK.Async;
using Dolittle.SDK.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Aggregates.Internal;

/// <summary>
/// Represents an implementation of <see cref="IAggregateRoots"/>.
/// </summary>
public class AggregateRoots : IAggregateRoots
{
    readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateRoots"/> class.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/>.</param>
    public AggregateRoots(ILogger logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public Try<TAggregate> TryGet<TAggregate>(EventSourceId eventSourceId, IServiceProvider provider)
        where TAggregate : AggregateRoot
    {
        _logger.Get(typeof(TAggregate), eventSourceId);
        if (!TryGetConstructor(typeof(TAggregate), out var constructor, out var exception))
        {
            return exception;
        }

        return CreateInstance<TAggregate>(eventSourceId, constructor, provider);
    }

    static Try<TAggregate> CreateInstance<TAggregate>(EventSourceId eventSourceId, ConstructorInfo constructor, IServiceProvider provider)
        where TAggregate : AggregateRoot
    {
        try
        {
            var aggregateRoot = HasEventSourceIdParameter(constructor) switch
            {
                true => ActivatorUtilities.CreateInstance<TAggregate>(provider, eventSourceId),
                false => ActivatorUtilities.CreateInstance<TAggregate>(provider)
            };
            aggregateRoot.EventSourceId = eventSourceId;
            return aggregateRoot;
        }
        catch (Exception ex)
        {
            return new CouldNotCreateAggregateRootInstance(typeof(TAggregate), eventSourceId, ex);
        }
    }

    static bool TryGetConstructor(Type type, [NotNullWhen(true)]out ConstructorInfo? constructor, [NotNullWhen(false)]out Exception? ex)
    {
        constructor = default;
        if (MoreThanOnePublicConstructor(type, out ex))
        {
            return false;
        }

        constructor = type.GetConstructors().SingleOrDefault() ?? type.GetConstructor(Type.EmptyTypes)!;
        return true;
    }

    static bool MoreThanOnePublicConstructor(Type type, [NotNullWhen(true)]out Exception? ex)
    {
        ex = default;
        if (type.GetConstructors().Length <= 1)
        {
            return false;
        }

        ex = new InvalidAggregateRootConstructorSignature(type, "expected at most a single public constructor");
        return true;
    }

    static bool HasEventSourceIdParameter(ConstructorInfo constructor)
        => constructor.GetParameters().Any(_ => _.ParameterType == typeof(EventSourceId));
}
