// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Common.Model;

/// <summary>
/// Represents a binding for an identifier to a processor builder.
/// </summary>
/// <param name="Identifier">The identifier that is bound.</param>
/// <param name="ProcessorBuilder">The processor builder that the identifier is bound to.</param>
/// <typeparam name="TBuilder">The <see cref="Type"/>of the processor builder.</typeparam>
public record ProcessorBuilderBinding<TBuilder>(IIdentifier Identifier, TBuilder ProcessorBuilder) : Binding(Identifier)
    where TBuilder : class
{
    /// <inheritdoc />
    public override string ToString()
        => $"processor Builder binding from {Identifier} to {ProcessorBuilder}";
}
