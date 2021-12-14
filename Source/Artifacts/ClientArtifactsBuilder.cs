// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dolittle.SDK.Common.ClientSetup;

namespace Dolittle.SDK.Artifacts;

/// <summary>
/// Represents an implementation of <see cref="IArtifactsBuilder{TArtifact,TId}"/>.
/// </summary>
/// <typeparam name="TArtifact">The <see cref="Type" /> of the <see cref="Artifact{TId}" />.</typeparam>
/// <typeparam name="TId">The <see cref="Type" /> of the <see cref="ArtifactId" />.</typeparam>
/// <typeparam name="TArtifactAttribute">The <see cref="Type"/> of the <see cref="Attribute"/> used to mark the artifact.</typeparam>
public abstract class ClientArtifactsBuilder<TArtifact, TId, TArtifactAttribute> : IArtifactsBuilder<TArtifact, TId>
    where TArtifact : Artifact<TId>
    where TId : ArtifactId
    where TArtifactAttribute : Attribute
{
    const string AttributeString = $"[{nameof(TArtifactAttribute)}(...)]";

    readonly Dictionary<TArtifact, HashSet<Type>> _artifactToTypeMap = new();
    readonly Dictionary<Type, HashSet<TArtifact>> _typeToArtifactMap = new();
    readonly IClientBuildResults _clientBuildResults;

    /// <summary>
    /// Initializes an instance of the <see cref="ClientArtifactsBuilder{TArtifact,TId,TArtifactAttribute}"/> class.
    /// </summary>
    /// <param name="clientBuildResults"></param>
    protected ClientArtifactsBuilder(IClientBuildResults clientBuildResults)
    {
        _clientBuildResults = clientBuildResults;
    }

    /// <inheritdoc />
    public void Associate(Type type, TArtifact artifact)
    {
        if (TryGetArtifactFromAttributeOnType(type, out var artifactFromAttribute)
            && !artifact.Equals(artifactFromAttribute))
        {
            _clientBuildResults.AddFailure(
                $"Trying to associate {type} with {artifact}, but it is already associated to {artifactFromAttribute}",
                $"Either the {AttributeString} on {type} is wrong and remove that or the manual association of {type} to {artifact} is wrong and remove that");
            return;
        }
        AddArtifactToTypeMapping(artifact, type);
        AddTypeToArtifactMapping(type, artifact);
    }

    /// <inheritdoc />
    public void Register(Type type)
    {
        if (TryGetArtifactFromAttributeOnType(type, out var artifact))
        {
            Associate(type, artifact);
        }
        else
        {
            _clientBuildResults.AddFailure(
                $"{type} is missing the {AttributeString} attribute",
                $"Put the {AttributeString} attribute on the ${type} class");
        }

    }

    /// <inheritdoc />
    public void RegisterAllFrom(Assembly assembly)
    {
        foreach (var type in assembly.ExportedTypes)
        {
            if (TryGetArtifactAttribute(type, out _))
            {
                Register(type);
            }
        }
    }


    /// <inheritdoc />
    public IDictionary<Type, TArtifact> Build() => _typeToArtifactMap
        .Where(_ => _.Value.Count == 1)
        .ToDictionary(_ => _.Key, _ => _.Value.First());


    /// <summary>
    /// Tries to get the <typeparamref name="TArtifact"/> from the <typeparamref name="TArtifactAttribute"/>.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> that the <typeparamref name="TArtifactAttribute"/> was on.</param>
    /// <param name="attribute">The <typeparamref name="TArtifactAttribute"/>.</param>
    /// <param name="artifact">The extracted <typeparamref name="TArtifact"/>.</param>
    /// <returns>The value indicating whether the <see cref="Type"/> is missing a valid <typeparamref name="TArtifactAttribute"/>.</returns>
    protected abstract bool TryGetArtifactFromAttribute(Type type, TArtifactAttribute attribute, out TArtifact artifact);

    void AddArtifactToTypeMapping(TArtifact artifact, Type type)
    {
        if (_artifactToTypeMap.TryGetValue(artifact, out var associatedTypes))
        {
            var newAssociatedTypes = associatedTypes.Append(type).ToHashSet();
            if (associatedTypes.Count < newAssociatedTypes.Count)
            {
                _clientBuildResults.AddFailure(
                    $"{artifact} is already associated with {string.Join(", ", associatedTypes)}",
                    $"Only associate {artifact} with one type. Maybe you are manually associating but have forgotten to turn of automatic discovery?");
            }
        }
        else
        {
            _artifactToTypeMap.Add(artifact, new HashSet<Type> { type });
        }
    }

    void AddTypeToArtifactMapping(Type type, TArtifact artifact)
    {
        if (_typeToArtifactMap.TryGetValue(type, out var associatedArtifacts))
        {
            var newAssociatedArtifacts = associatedArtifacts.Append(artifact).ToHashSet();
            if (associatedArtifacts.Count < newAssociatedArtifacts.Count)
            {
                _clientBuildResults.AddFailure(
                    $"{type} is already associated with {string.Join(", ", associatedArtifacts)}",
                    $"Only associate {type} with one {nameof(TArtifact)}. Maybe you are manually associating but have forgotten to turn of automatic discovery? Or maybe {type} has an {AttributeString} attribute defining another {nameof(TArtifact)}");
            }
        }
        else
        {
            _typeToArtifactMap.Add(type, new HashSet<TArtifact> { artifact });
        }
    }

    bool TryGetArtifactFromAttributeOnType(Type type, out TArtifact artifact)
    {
        artifact = default;
        return TryGetArtifactAttribute(type, out var attribute) && TryGetArtifactFromAttribute(type, attribute, out artifact);
    }

    static bool TryGetArtifactAttribute(Type type, out TArtifactAttribute attribute)
    {
        attribute = default;
        if (Attribute.GetCustomAttribute(type, typeof(TArtifactAttribute)) is not TArtifactAttribute attributeOnType)
        {
            return false;
        }
        attribute = attributeOnType;
        return true;
    }
}
