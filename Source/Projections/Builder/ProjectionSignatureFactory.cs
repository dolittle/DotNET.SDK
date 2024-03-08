// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Projections.Builder;

enum ProjectionMethodResponseType
{
    Void,
    ProjectionResult,
    ProjectionResultType
}

enum ProjectionParametersType
{
    EventOnly,
    EventAndProjectionContext,
    EventAndEventContext
}

static class ProjectionSignatureFactory<TProjection>
    where TProjection : ReadModel, new()
{
    /// <summary>
    /// Tries to map the given <see cref="MethodInfo" /> to a <see cref="ProjectionSignature{TProjection, TEvent}" />.
    /// Throws if it does not match any of the expected signatures.
    /// </summary>
    /// <param name="method"></param>
    /// <typeparam name="TEvent"></typeparam>
    /// <returns></returns>
    public static ProjectionSignature<TProjection, TEvent> TryMap<TEvent>(MethodInfo method)
        where TEvent : class
    {
        if (!ResponseTypeIsValid(method, out var responseType))
        {
            throw new ArgumentException("Return type must be void, ProjectionResult or ProjectionResultType");
        }

        if (!ParametersAreValid(method, out var parametersType))
        {
            throw new ArgumentException("Parameters must be an event, optionally an event and an EventContext or ProjectionContext");
        }

        return GetTypedDelegate<TEvent>(method, parametersType.Value, responseType.Value);
    }

    /// <summary>
    /// Tries to map the given <see cref="MethodInfo" /> to a <see cref="ProjectionSignature{TProjection}" />.
    /// Throws if it does not match any of the expected signatures.
    /// </summary>
    /// <param name="method"></param>
    /// <returns></returns>
    public static ProjectionSignature<TProjection> MapUnTyped(MethodInfo method)
    {
        if (!ResponseTypeIsValid(method, out var responseType))
        {
            throw new ArgumentException("Return type must be void, ProjectionResult or ProjectionResultType");
        }

        if (!ParametersAreValid(method, out var parametersType))
        {
            throw new ArgumentException("Parameters must be an event, optionally an event and an EventContext or ProjectionContext");
        }

        return GetUnTypedDelegate(method, parametersType.Value, responseType.Value);
    }


    static ProjectionSignature<TProjection, TEvent> GetTypedDelegate<TEvent>(MethodInfo method, ProjectionParametersType parametersType,
        ProjectionMethodResponseType responseType) where TEvent : class
    {
        return responseType switch
        {
            ProjectionMethodResponseType.Void => VoidReturnType<TEvent>(method, parametersType),
            ProjectionMethodResponseType.ProjectionResult => ProjectionResultReturn<TEvent>(method, parametersType),
            ProjectionMethodResponseType.ProjectionResultType => ResponseTypeReturn<TEvent>(method, parametersType),
            _ => throw new ArgumentOutOfRangeException(nameof(responseType), responseType, null)
        };
    }

    static ProjectionSignature<TProjection> GetUnTypedDelegate(MethodInfo method, ProjectionParametersType parametersType,
        ProjectionMethodResponseType responseType)
    {
        return responseType switch
        {
            ProjectionMethodResponseType.Void => VoidReturnType(method, parametersType),
            ProjectionMethodResponseType.ProjectionResult => ProjectionResultReturn(method, parametersType),
            ProjectionMethodResponseType.ProjectionResultType => ResponseTypeReturn(method, parametersType),
            _ => throw new ArgumentOutOfRangeException(nameof(responseType), responseType, null)
        };
    }


    static ProjectionSignature<TProjection, TEvent> ProjectionResultReturn<TEvent>(MethodInfo method, ProjectionParametersType parametersType)
        where TEvent : class
    {
        return parametersType switch
        {
            ProjectionParametersType.EventOnly
                => ProjectionSignatureMapper.Map(ToDelegate<ProjectionEventSignature<TProjection, TEvent>>(method)),

            ProjectionParametersType.EventAndProjectionContext
                => ToDelegate<ProjectionSignature<TProjection, TEvent>>(method),

            ProjectionParametersType.EventAndEventContext
                => ProjectionSignatureMapper.Map(ToDelegate<ProjectionEventContextSignature<TProjection, TEvent>>(method)),

            _ => throw new ArgumentOutOfRangeException(nameof(parametersType), parametersType, null)
        };
    }

    static ProjectionSignature<TProjection> ProjectionResultReturn(MethodInfo method, ProjectionParametersType parametersType)
    {
        return parametersType switch
        {
            ProjectionParametersType.EventOnly
                => ProjectionSignatureMapper.Map(ToDelegate<ProjectionEventSignature<TProjection>>(method)),

            ProjectionParametersType.EventAndProjectionContext
                => ToDelegate<ProjectionSignature<TProjection>>(method),

            ProjectionParametersType.EventAndEventContext
                => ProjectionSignatureMapper.Map(ToDelegate<ProjectionEventContextSignature<TProjection>>(method)),

            _ => throw new ArgumentOutOfRangeException(nameof(parametersType), parametersType, null)
        };
    }

    static ProjectionSignature<TProjection, TEvent> ResponseTypeReturn<TEvent>(MethodInfo method, ProjectionParametersType parametersType) where TEvent : class
    {
        return parametersType switch
        {
            ProjectionParametersType.EventOnly
                => ProjectionSignatureMapper.Map(ToDelegate<ProjectionResultTypeEventSignature<TProjection, TEvent>>(method)),

            ProjectionParametersType.EventAndProjectionContext
                => ProjectionSignatureMapper.Map(ToDelegate<ProjectionResultTypeSignature<TProjection, TEvent>>(method)),

            ProjectionParametersType.EventAndEventContext
                => ProjectionSignatureMapper.Map(ToDelegate<ProjectionResultTypeEventContextSignature<TProjection, TEvent>>(method)),

            _ => throw new ArgumentOutOfRangeException(nameof(parametersType), parametersType, null)
        };
    }

    static ProjectionSignature<TProjection> ResponseTypeReturn(MethodInfo method, ProjectionParametersType parametersType)
    {
        return parametersType switch
        {
            ProjectionParametersType.EventOnly
                => ProjectionSignatureMapper.Map(ToDelegate<ProjectionResultTypeEventSignature<TProjection>>(method)),

            ProjectionParametersType.EventAndProjectionContext
                => ProjectionSignatureMapper.Map(ToDelegate<ProjectionResultTypeSignature<TProjection>>(method)),

            ProjectionParametersType.EventAndEventContext
                => ProjectionSignatureMapper.Map(ToDelegate<ProjectionResultTypeEventContextSignature<TProjection>>(method)),

            _ => throw new ArgumentOutOfRangeException(nameof(parametersType), parametersType, null)
        };
    }

    static ProjectionSignature<TProjection, TEvent> VoidReturnType<TEvent>(MethodInfo method, ProjectionParametersType parametersType) where TEvent : class
    {
        return parametersType switch
        {
            ProjectionParametersType.EventOnly
                => ProjectionSignatureMapper.Map(ToDelegate<ProjectionMethodEventSignature<TProjection, TEvent>>(method)),

            ProjectionParametersType.EventAndProjectionContext
                => ProjectionSignatureMapper.Map(ToDelegate<ProjectionMethodSignature<TProjection, TEvent>>(method)),

            ProjectionParametersType.EventAndEventContext
                => ProjectionSignatureMapper.Map(ToDelegate<ProjectionMethodEventContextSignature<TProjection, TEvent>>(method)),

            _ => throw new ArgumentOutOfRangeException(nameof(parametersType), parametersType, null)
        };
    }

    static ProjectionSignature<TProjection> VoidReturnType(MethodInfo method, ProjectionParametersType parametersType)
    {
        return parametersType switch
        {
            ProjectionParametersType.EventOnly
                => ProjectionSignatureMapper.Map(ToDelegate<ProjectionMethodEventSignature<TProjection>>(method)),

            ProjectionParametersType.EventAndProjectionContext
                => ProjectionSignatureMapper.Map(ToDelegate<ProjectionMethodSignature<TProjection>>(method)),

            ProjectionParametersType.EventAndEventContext
                => ProjectionSignatureMapper.Map(ToDelegate<ProjectionMethodEventContextSignature<TProjection>>(method)),

            _ => throw new ArgumentOutOfRangeException(nameof(parametersType), parametersType, null)
        };
    }


    static T ToDelegate<T>(MethodInfo method) where T : Delegate
    {
        return (T)Delegate.CreateDelegate(typeof(T), method);
    }

    internal static bool ParametersAreValid(MethodInfo method, [NotNullWhen(true)] out ProjectionParametersType? parametersType)
    {
        var parameters = method.GetParameters();
        parametersType = default;
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
            parametersType = ProjectionParametersType.EventOnly;
            return true;
        }

        var secondParameterType = parameters[1].ParameterType;

        switch (secondParameterType)
        {
            // EventContext
            case { } type when type == typeof(EventContext):
                parametersType = ProjectionParametersType.EventAndEventContext;
                return true;
            // ProjectionContext
            case { } type when type == typeof(ProjectionContext):
                parametersType = ProjectionParametersType.EventAndProjectionContext;
                return true;
            default:
                // Invalid second parameter
                return false;
        }
    }

    internal static bool ResponseTypeIsValid(MethodInfo method, [NotNullWhen(true)] out ProjectionMethodResponseType? projectionMethodResponseType)
    {
        switch (method.ReturnType)
        {
            case { } returnType when returnType == typeof(void):
                projectionMethodResponseType = ProjectionMethodResponseType.Void;
                return true;
            // ProjectionResultType
            case { } returnType when returnType == typeof(ProjectionResultType):
                projectionMethodResponseType = ProjectionMethodResponseType.ProjectionResultType;
                return true;
            case { } returnType when returnType == typeof(ProjectionResult<TProjection>):
                projectionMethodResponseType = ProjectionMethodResponseType.ProjectionResult;
                return true;
            // case { } returnType when returnType == typeof(TProjection):
            //     projectionMethodResponseType = ProjectionMethodResponseType.ProjectionValue;
            //     return true;
            default:
                projectionMethodResponseType = default;
                return false;
        }
    }
}
