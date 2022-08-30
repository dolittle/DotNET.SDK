// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
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
    /// <param name="id">When the method returns, the converted <typeparamref name="TId"/> if conversion was successful, otherwise default.</param>
    /// <param name="generation">When the method returns, the converted <see cref="Generation"/> if conversion was successful, otherwise default.</param>
    /// <param name="error">When the method returns, null if the conversion was successful, otherwise the error that caused the failure.</param>
    /// <typeparam name="TId">The <see cref="Type" /> of the <see cref="ArtifactId" />.</typeparam>
    /// <returns>A value indicating whether or not the conversion was successful.</returns>
    public static bool TryToArtifact<TId>(this PbArtifact source, [NotNullWhen(true)]out TId? id, [NotNullWhen(true)]out Generation? generation, [NotNullWhen(false)]out Exception? error)
        where TId : ArtifactId
    {
        generation = default;
        if (!source.Id.TryTo<TId>(out id, out error))
        {
            error = new CouldNotConvertProtobufArtifact(typeof(TId), source, error.Message);
            return false;
        }
        
        generation = source.Generation;
        return true;
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
    public static (TId Id, Generation Generation) ToArtifact<TId>(this PbArtifact source)
        where TId : ArtifactId
        => source.TryToArtifact<TId>(out var id, out var generation, out var error)
            ? (id, generation)
            : throw error;
}
