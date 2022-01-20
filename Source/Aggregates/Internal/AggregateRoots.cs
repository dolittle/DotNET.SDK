// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Reflection;
using Dolittle.SDK.Async;
using Dolittle.SDK.Events;
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
    public Try<TAggregate> TryGet<TAggregate>(EventSourceId eventSourceId)
        where TAggregate : AggregateRoot
    {
        _logger.LogDebug(
            "Getting aggregate root {AggregateRoot} with event source id {EventSource}",
            typeof(TAggregate),
            eventSourceId);
        if (InvalidConstructor(typeof(TAggregate), out var constructor, out var exception))
        {
            return exception;
        }

        return CreateInstance<TAggregate>(eventSourceId, constructor);
    }

    static Try<TAggregate> CreateInstance<TAggregate>(EventSourceId eventSourceId, ConstructorInfo constructor)
        where TAggregate : AggregateRoot
    {
        var aggregateRoot = constructor.Invoke(new object[] { eventSourceId }) as TAggregate;
        if (CouldNotCreateAggregateRoot(aggregateRoot, out var exception))
        {
            return exception;
        }

        return aggregateRoot;
    }

    static bool InvalidConstructor(Type type, out ConstructorInfo constructor, out Exception ex)
    {
        constructor = default;
        if (NotOnlyOneConstructor(type, out ex))
        {
            return true;
        }

        constructor = type.GetConstructors().Single();
        return ConstructorIsInvalid(type, constructor, out ex);
    }

    static bool NotOnlyOneConstructor(Type type, out Exception ex)
    {
        ex = default;
        if (ThereIsOnlyOneConstructor(type))
        {
            return false;
        }

        ex = new InvalidAggregateRootConstructorSignature(type, "expected only a single constructor");
        return true;
    }

    static bool ConstructorIsInvalid(Type type, ConstructorInfo constructor, out Exception ex)
        => IncorrectParameter(type, constructor.GetParameters(), out ex);

    static bool IncorrectParameter(Type type, ParameterInfo[] parameters, out Exception ex)
    {
        ex = default;
        if (ThereIsOnlyOneParameter(parameters) && ParameterTypeIsGuidOrEventSourceId(parameters))
        {
            return false;
        }

        ex = new InvalidAggregateRootConstructorSignature(type, $"expected only one parameter and it must be of type {typeof(Guid)} or {typeof(EventSourceId)}");
        return true;
    }

    static bool CouldNotCreateAggregateRoot<TAggregate>(TAggregate aggregateRoot, out Exception ex)
        where TAggregate : AggregateRoot
    {
        ex = default;
        if (aggregateRoot != default)
        {
            return false;
        }

        ex = new CouldNotCreateAggregateRootInstance(typeof(TAggregate));
        return true;
    }

    static bool ThereIsOnlyOneConstructor(Type type) => type.GetConstructors().Length == 1;

    static bool ThereIsOnlyOneParameter(ParameterInfo[] parameters) => parameters.Length == 1;

    static bool ParameterTypeIsGuidOrEventSourceId(ParameterInfo[] parameters)
        => parameters[0].ParameterType == typeof(Guid) || parameters[0].ParameterType == typeof(EventSourceId);
}
