// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Events;
using Dolittle.SDK.Projections.Builder.Copies.MongoDB;
using Dolittle.SDK.Projections.Copies;

namespace Dolittle.SDK.Projections.Builder;

/// <summary>
/// Methods for building <see cref="IProjection{TReadModel}"/> instances by convention from an instantiated projection class.
/// </summary>
/// <typeparam name="TProjection">The <see cref="Type" /> of the read model.</typeparam>
public class ConventionProjectionBuilder<TProjection> : ICanTryBuildProjection
    where TProjection : class, new()
{
    readonly ProjectionAttribute _decorator;
    readonly Copies.MongoDB.Internal.IProjectionCopyToMongoDBBuilder<TProjection> _copyToMongoDbBuilder;
    const string MethodName = "On";
    readonly Type _projectionType = typeof(TProjection);

    /// <summary>
    /// Initializes an instance of the <see cref="ConventionProjectionBuilder{TProjection}"/> class.
    /// </summary>
    /// <param name="decorator">The <see cref="ProjectionAttribute"/> decorator.</param>
    /// <param name="copyToMongoDbBuilder">The <see cref="Copies.MongoDB.Internal.IProjectionCopyToMongoDBBuilder{TProjection}"/>.</param>
    public ConventionProjectionBuilder(ProjectionAttribute decorator, Copies.MongoDB.Internal.IProjectionCopyToMongoDBBuilder<TProjection> copyToMongoDbBuilder)
    {
        _decorator = decorator;
        _copyToMongoDbBuilder = copyToMongoDbBuilder;
    }

    /// <inheritdoc />
    public bool Equals(ICanTryBuildProjection other)
    {
        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return other is ConventionProjectionBuilder<TProjection> otherBuilder
            && _projectionType == otherBuilder._projectionType;
    }
    
    /// <inheritdoc/>
    public bool TryBuild(IEventTypes eventTypes, IClientBuildResults buildResults, out IProjection projection)
    {
        projection = default;
        buildResults.AddInformation($"Building projection {_decorator.Identifier} processing events in scope {_decorator.Scope} from type {_projectionType}");
        
        if (!HasParameterlessConstructor())
        {
            buildResults.AddFailure($"The projection class {_projectionType} has no default/parameterless constructor");
            return false;
        }
        
        if (HasMoreThanOneConstructor())
        {
            buildResults.AddFailure($"The projection class {_projectionType} has more than one constructor", "It must only have one, parameterless, constructor");
            return false;
        }

        var eventTypesToMethods = new Dictionary<EventType, IProjectionMethod<TProjection>>();
        if (!TryBuildOnMethods(eventTypes, eventTypesToMethods, buildResults))
        {
            return false;
        }
        if (!_copyToMongoDbBuilder.TryBuildFromReadModel(buildResults, out var copyToMongoDB))
        {
            buildResults.AddFailure($"Failed to build projection copies definition for projection {_decorator.Identifier} using conventions from projection type {_projectionType}");
            return false;
        }
        
        projection = new Projection<TProjection>(_decorator.Identifier, _decorator.Scope, eventTypesToMethods, new ProjectionCopies(copyToMongoDB));
        return true;
    }
    
    
    bool HasParameterlessConstructor()
        => _projectionType.GetConstructors().Any(t => t.GetParameters().Length == 0);

    bool HasMoreThanOneConstructor() => _projectionType.GetConstructors().Length > 1;
    
    bool TryBuildOnMethods(
        IEventTypes eventTypes,
        IDictionary<EventType, IProjectionMethod<TProjection>> eventTypesToMethods,
        IClientBuildResults buildResults)
    {
        var allMethods = _projectionType.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic);
        var hasWrongMethods = !TryAddDecoratedOnMethods(allMethods, eventTypesToMethods, buildResults)
            || !TryAddConventionOnMethods(allMethods, eventTypes, eventTypesToMethods, buildResults);

        if (hasWrongMethods)
        {
            return false;
        }

        if (eventTypesToMethods.Any())
        {
            return true;
        }
        buildResults.AddFailure($"There are no projection methods to register in projection {_projectionType}. A projection method either needs to be decorated with [{nameof(OnAttribute)}] or have the name {MethodName}");
        return false;

    }

    bool TryAddDecoratedOnMethods(
        IEnumerable<MethodInfo> methods,
        IDictionary<EventType, IProjectionMethod<TProjection>> eventTypesToMethods,
        IClientBuildResults buildResults)
    {
        var allMethodsAdded = true;
        foreach (var method in methods.Where(IsDecoratedOnMethod))
        {
            var shouldAddHandler = true;
            var eventType = (method.GetCustomAttributes(typeof(OnAttribute), true)[0] as OnAttribute)?.EventType;

            if (!TryGetKeySelector(method, buildResults, out var keySelector))
            {
                shouldAddHandler = false;
            }

            if (!TryGetEventParameterType(method, out var eventParameterType))
            {
                shouldAddHandler = false;
                buildResults.AddFailure($"Projection method {method} on projection {_projectionType} has no parameters, but is decorated with [{nameof(OnAttribute)}]. A projection method should take in as parameters an event and a {nameof(ProjectionContext)}");
            }

            if (!ParametersAreOkay(method, buildResults))
            {
                shouldAddHandler = false;
            }

            if (eventParameterType != typeof(object))
            {
                shouldAddHandler = false;
                buildResults.AddFailure($"Projection method {method} on projection {_projectionType} should only handle an event of type object");
            }

            if (!method.IsPublic)
            {
                buildResults.AddFailure($"Method {method} on projection {_projectionType} has the signature of an projection method, but is not public. Projection methods needs to be public");
                shouldAddHandler = false;
            }

            if (!shouldAddHandler)
            {
                allMethodsAdded = false;
                continue;
            }
            if (eventTypesToMethods.TryAdd(eventType, CreateUntypedOnMethod(method, eventType, keySelector)))
            {
                continue;
            }
            allMethodsAdded = false;
            buildResults.AddFailure($"Event type {eventType} is already handled in projection {_decorator.Identifier}");
        }

        return allMethodsAdded;
    }

    bool TryAddConventionOnMethods(
        IEnumerable<MethodInfo> methods,
        IEventTypes eventTypes,
        IDictionary<EventType, IProjectionMethod<TProjection>> eventTypesToMethods,
        IClientBuildResults buildResults)
    {
        var allMethodsAdded = true;
        foreach (var method in methods.Where(_ => !IsDecoratedOnMethod(_) && _.Name == MethodName))
        {
            var shouldAddHandler = TryGetKeySelector(method, buildResults, out var keySelector);

            if (!TryGetEventParameterType(method, out var eventParameterType))
            {
                shouldAddHandler = false;
                buildResults.AddFailure($"Projection method {method} on projection {_projectionType} has no parameters. An projection method should take in as parameters an event and an {nameof(ProjectionContext)}");
            }

            if (eventParameterType == typeof(object))
            {
                shouldAddHandler = false;
                buildResults.AddFailure($"Projection method {method} on projection {_projectionType} cannot handle an untyped event when not decorated with [{nameof(OnAttribute)}]");
            }

            if (!eventTypes.HasFor(eventParameterType))
            {
                shouldAddHandler = false;
                buildResults.AddFailure($"Projection method {method} on projection {_projectionType} handles event of type {eventParameterType}, but it is not associated to any event type");
            }

            if (!ParametersAreOkay(method, buildResults))
            {
                shouldAddHandler = false;
            }

            if (!method.IsPublic)
            {
                shouldAddHandler = false;
                buildResults.AddFailure($"Method {method} on projection {_projectionType} has the signature of an projection method, but is not public. Projection methods needs to be public");
            }

            if (!shouldAddHandler)
            {
                allMethodsAdded = false;
                continue;
            }

            var eventType = eventTypes.GetFor(eventParameterType);
            if (eventTypesToMethods.TryAdd(eventType, CreateTypedOnMethod(eventParameterType, method, keySelector)))
            {
                continue;
            }
            allMethodsAdded = false;
            buildResults.AddFailure($"Event type {eventParameterType} is already handled in projection {_decorator.Identifier}");
        }

        return allMethodsAdded;
    }

    IProjectionMethod<TProjection> CreateUntypedOnMethod(MethodInfo method, EventType eventType, KeySelector keySelector)
    {
        var projectionSignatureType = GetSignature(method);
        var projectionSignature = method.CreateDelegate(projectionSignatureType.MakeGenericType(_projectionType), null);
        return Activator.CreateInstance(
            typeof(ClassProjectionMethod<>).MakeGenericType(_projectionType),
            projectionSignature,
            eventType,
            keySelector) as IProjectionMethod<TProjection>;
    }

    IProjectionMethod<TProjection> CreateTypedOnMethod(Type eventParameterType, MethodInfo method, KeySelector keySelector)
    {
        var projectionSignatureGenericTypeDefinition = GetTypedSignature(method);
        var projectionSignatureType = projectionSignatureGenericTypeDefinition.MakeGenericType(_projectionType, eventParameterType);
        var projectionSignature = method.CreateDelegate(projectionSignatureType, null);

        return Activator.CreateInstance(
            typeof(TypedClassProjectionMethod<,>).MakeGenericType(_projectionType, eventParameterType),
            projectionSignature,
            keySelector) as IProjectionMethod<TProjection>;
    }

    static Type GetSignature(MethodInfo method)
    {
        if (MethodReturnsTask(method))
        {
            return typeof(TaskProjectionMethodSignature<>);
        }
        if (MethodReturnsTaskResultType(method))
        {
            return typeof(TaskResultProjectionMethodSignature<>);
        }
        if (MethodReturnsVoid(method))
        {
            return typeof(SyncProjectionMethodSignature<>);
        }
        if (MethodReturnsResultType(method))
        {
            return typeof(SyncResultProjectionMethodSignature<>);
        }
        throw new InvalidProjectionMethodReturnType(method.ReturnType);
    }

    static Type GetTypedSignature(MethodInfo method)
    {
        if (MethodReturnsTask(method))
        {
            return typeof(TaskProjectionMethodSignature<,>);
        }
        if (MethodReturnsTaskResultType(method))
        {
            return typeof(TaskResultProjectionMethodSignature<,>);
        }
        if (MethodReturnsVoid(method))
        {
            return typeof(SyncProjectionMethodSignature<,>);
        }
        if (MethodReturnsResultType(method))
        {
            return typeof(SyncResultProjectionMethodSignature<,>);
        }
        throw new InvalidProjectionMethodReturnType(method.ReturnType);
    }

    bool ParametersAreOkay(MethodInfo method, IClientBuildResults buildResults)
    {
        var okay = true;
        if (!SecondMethodParameterIsProjectionContext(method))
        {
            okay = false;
            buildResults.AddFailure($"Projection method {method} on projection {_projectionType} needs to have two parameters where the second parameter is {typeof(ProjectionContext)}");
        }

        if (!MethodHasNoExtraParameters(method))
        {
            okay = false;
            buildResults.AddFailure($"Projection method {method} on projection {_projectionType} needs to only have two parameters where the first is the event to handle and the second is {typeof(ProjectionContext)}");
        }

        if (!MethodReturnsAsyncVoid(method) && (MethodReturnsVoid(method) || MethodReturnsResultType(method) || MethodReturnsTask(method) || MethodReturnsTaskResultType(method)))
        {
            return okay;
        }
        buildResults.AddFailure($"Projection method {method} on projection {_projectionType} needs to return either {typeof(void)}, {typeof(ProjectionResultType)}, {typeof(Task)}, {typeof(Task<ProjectionResultType>)}");
        return false;
    }

    bool TryGetKeySelector(MethodInfo method, IClientBuildResults buildResults, out KeySelector keySelector)
    {
        keySelector = null;
        var attributes = method
            .GetCustomAttributes()
            .OfType<IKeySelectorAttribute>().ToArray();
        if (attributes.Length > 1)
        {
            buildResults.AddFailure($"Method {method} on projection {_projectionType} has more than one key selector attributes", "Use only one key selector");
            return false;
        }

        if (!attributes.Any())
        {
            buildResults.AddFailure($"Method {method} on projection {_projectionType} has no key selector attribute", $"Add a key selector attribute: [{nameof(KeyFromPartitionAttribute)}], [{nameof(KeyFromPropertyAttribute)}] or [{nameof(KeyFromEventSourceAttribute)}],");
            return false;
        }

        keySelector = attributes.Single().KeySelector;

        return true;
    }

    static bool TryGetEventParameterType(MethodInfo method, out Type type)
    {
        type = default;
        if (method.GetParameters().Length == 0)
        {
            return false;
        }

        type = method.GetParameters()[0].ParameterType;
        return true;
    }

    static bool IsDecoratedOnMethod(MethodInfo method)
        => method.GetCustomAttributes(typeof(OnAttribute), true).FirstOrDefault() != default;

    static bool SecondMethodParameterIsProjectionContext(MethodInfo method)
        => method.GetParameters().Length > 1 && method.GetParameters()[1].ParameterType == typeof(ProjectionContext);

    static bool MethodHasNoExtraParameters(MethodInfo method)
        => method.GetParameters().Length == 2;

    static bool MethodReturnsTask(MethodInfo method)
        => method.ReturnType == typeof(Task);

    static bool MethodReturnsTaskResultType(MethodInfo method)
        => method.ReturnType == typeof(Task<ProjectionResultType>);

    static bool MethodReturnsVoid(MethodInfo method)
        => method.ReturnType == typeof(void);

    static bool MethodReturnsResultType(MethodInfo method)
        => method.ReturnType == typeof(ProjectionResultType);

    static bool MethodReturnsAsyncVoid(MethodInfo method)
    {
        var asyncAttribute = typeof(AsyncStateMachineAttribute);
        var isAsyncMethod = (AsyncStateMachineAttribute)method.GetCustomAttribute(asyncAttribute) != null;
        return isAsyncMethod && MethodReturnsVoid(method);
    }
}
