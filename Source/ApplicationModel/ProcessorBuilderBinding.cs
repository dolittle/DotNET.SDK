// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Concepts;

namespace Dolittle.SDK.ApplicationModel;

/// <summary>
/// Represents a binding for an identifier to a processor builder.
/// </summary>
/// <param name="Identifier">The identifier that is bound.</param>
/// <param name="ProcessorBuilder">The processor builder that the identifier is bound to.</param>
/// <typeparam name="TBuilder">The <see cref="Type"/>of the processor builder.</typeparam>
/// <typeparam name="TIdentifier">The <see cref="Type"/> of the <see cref="IIdentifier{TId}"/>.</typeparam>
/// <typeparam name="TId">The type of the globally unique id of the identifier.</typeparam>
public record ProcessorBuilderBinding<TBuilder, TIdentifier, TId>(TIdentifier Identifier, TBuilder ProcessorBuilder) : Binding<TIdentifier, TId>(Identifier)
    where TBuilder : IProcessorBuilder<TIdentifier, TId >
    where TIdentifier : IIdentifier<TId>
    where TId : ConceptAs<Guid>
{
    /// <inheritdoc />
    public override string ToString()
        => $"Processor Builder binding from {Identifier} to {ProcessorBuilder}";
}
