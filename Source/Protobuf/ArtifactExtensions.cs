// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Artifacts;
using PbArtifact = Dolittle.Artifacts.Contracts.Artifact;

namespace Dolittle.SDK.Protobuf;

/// <summary>
/// Conversion extensions for converting between <see cref="Artifact{TId}"/> and <see cref="PbArtifact"/>.
/// </summary>
public static class ArtifactExtensions
{
    /// <summary>
    /// Convert a <see cref="Artifact{TId}"/> to a <see cref="PbArtifact"/>.
    /// </summary>
    /// <param name="artifact"><see cref="Artifact{TId}"/> to convert.</param>
    /// <typeparam name="TId">The <see cref="Type" /> of the <see cref="ArtifactId" />.</typeparam>
    /// <returns>The converted <see cref="PbArtifact"/>.</returns>
    public static PbArtifact ToProtobuf<TId>(this Artifact<TId> artifact)
        where TId : ArtifactId
        => new() { Id = artifact.Id.ToProtobuf(), Generation = artifact.Generation.Value };

    /// <summary>
    /// Convert a <see cref="PbArtifact"/> to a <typeparamref name="TArtifact"/>.
    /// </summary>
    /// <remarks>
    /// The method assumes that the <see cref="Artifact{TId}" /> class has a constructor that takes in
    /// <see cref="ArtifactId" /> as first parameter and <see cref="Generation" /> as second parameter.
    /// It also assumes that the <see cref="ArtifactId" /> type in the <see cref="Artifact{TId}" /> only has the default parameterless constructor.
    /// </remarks>
    /// <param name="source"><see cref="PbArtifact"/> to convert.</param>
    /// <param name="artifact">When the method returns, the converted <see cref="Artifact{TId}"/> if conversion was successful, otherwise default.</param>
    /// <param name="error">When the method returns, null if the conversion was successful, otherwise the error that caused the failure.</param>
    /// <typeparam name="TArtifact">The <see cref="Type" /> of the <see cref="Artifact{TId}" />.</typeparam>
    /// <typeparam name="TId">The <see cref="Type" /> of the <see cref="ArtifactId" />.</typeparam>
    /// <returns>A value indicating whether or not the conversion was successful.</returns>
    public static bool TryTo<TArtifact, TId>(this PbArtifact source, out TArtifact artifact, out Exception error)
        where TArtifact : Artifact<TId>
        where TId : ArtifactId
    {
        artifact = default;

        if (source == default)
        {
            error = new ArgumentNullException(nameof(source));
            return false;
        }

        if (!source.Id.TryToGuid(out var id, out var idError))
        {
            error = new CouldNotConvertProtobufArtifact(typeof(TArtifact), source, idError.Message);
            return false;
        }

        var idType = typeof(TArtifact).GetProperty(nameof(Artifact<TId>.Id)).PropertyType;
        var generationType = typeof(TArtifact).GetProperty(nameof(Artifact<TId>.Generation)).PropertyType;
        try
        {
            var artifactId = Activator.CreateInstance(idType);
            var artifactIdValueProperty = idType.GetProperty(nameof(ArtifactId.Value));
            artifactIdValueProperty.SetValue(artifactId, id);

            var constructor = typeof(TArtifact).GetConstructor(new[] { idType, generationType });
            if (constructor == default)
            {
                error = new ArtifactTypeDoesNotHaveConstructorWithIdAndGeneration(typeof(TArtifact), source);
                return false;
            }

            artifact = constructor.Invoke(new object[] { artifactId, (Generation)source.Generation }) as TArtifact;
            if (artifact == default)
            {
                error = new CouldNotConvertProtobufArtifact(typeof(TArtifact), source, "Could not create instance of artifact");
                return false;
            }

            error = null;
            return true;
        }
        catch (MissingMethodException)
        {
            error = new ArtifactIdTypeDoesNotHaveDefaultParameterlessConstructor(typeof(TArtifact), source, idType);
            return false;
        }
        catch (Exception ex)
        {
            error = new CouldNotConvertProtobufArtifact(typeof(TArtifact), source, ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Convert a <see cref="PbArtifact"/> to a <typeparamref name="TArtifact"/>.
    /// </summary>
    /// <remarks>
    /// The method assumes that the <see cref="Artifact{TId}" /> class has a constructor that takes in
    /// <see cref="ArtifactId" /> as first parameter and <see cref="Generation" /> as second parameter.
    /// It also assumes that the <see cref="ArtifactId" /> type in the <see cref="Artifact{TId}" /> only has the default parameterless constructor.
    /// </remarks>
    /// <param name="source"><see cref="PbArtifact"/> to convert.</param>
    /// <typeparam name="TArtifact">The <see cref="Type" /> of the <see cref="Artifact{TId}" />.</typeparam>
    /// <typeparam name="TId">The <see cref="Type" /> of the <see cref="ArtifactId" />.</typeparam>
    /// <returns>The converted <see cref="Artifact{TId}"/>.</returns>
    public static TArtifact To<TArtifact, TId>(this PbArtifact source)
        where TArtifact : Artifact<TId>
        where TId : ArtifactId
        => source.TryTo<TArtifact, TId>(out var artifact, out var error)
            ? artifact
            : throw error;

    /// <summary>
    /// Convert a <see cref="PbArtifact"/> to a tuple with an <see cref="ArtifactId" /> and a <see cref="Generation" />.
    /// </summary>
    /// <param name="source"><see cref="PbArtifact"/> to convert.</param>
    /// <param name="artifact">When the method returns, a tuple with <see cref="ArtifactId" /> and <see cref="Generation" /> if conversion was successful, otherwise null.</param>
    /// <param name="error">When the method returns, null if the conversion was successful, otherwise the error that caused the failure.</param>
    /// <returns>A value indicating whether or not the conversion was successful.</returns>
    public static bool TryToArtifact(this PbArtifact source, out (ArtifactId Id, Generation Generation) artifact, out Exception error)
    {
        artifact = default;
        if (!source.Id.TryTo<ArtifactId>(out var id, out error))
        {
            return false;
        }

        artifact = (id, source.Generation);
        return true;
    }

    /// <summary>
    /// Convert a <see cref="PbArtifact"/> to a tuple with an <see cref="ArtifactId" /> and a <see cref="Generation" />.
    /// </summary>
    /// <param name="source"><see cref="PbArtifact"/> to convert.</param>
    /// <returns>A tuple with <see cref="ArtifactId" /> and <see cref="Generation" />.</returns>
    public static (ArtifactId Id, Generation Generation) ToArtifact(this PbArtifact source)
        => source.TryToArtifact(out var artifact, out var error)
            ? artifact
            : throw error;
}
