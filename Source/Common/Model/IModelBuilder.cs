// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Concepts;

namespace Dolittle.SDK.Common.Model;

/// <summary>
/// Defines a builder for building an <see cref="IModel"/>.
/// </summary>
public interface IModelBuilder
{
    /// <summary>
    /// Adds a binding between an identifier and a type.
    /// </summary>
    /// <param name="identifier">The identifier to bind.</param>
    /// <param name="type">The type to bind the identifier to.</param>
    /// <typeparam name="TIdentifier">The <see cref="Type"/> of the <see cref="IIdentifier{TId}"/>.</typeparam>
    /// <typeparam name="TId">The type of the globally unique id of the identifier.</typeparam>
    void BindIdentifierToType<TIdentifier, TId>(TIdentifier identifier, Type type)
        where TIdentifier : IIdentifier<TId>
        where TId : ConceptAs<Guid>;
    
    /// <summary>
    /// Removes a binding between an identifier and a type.
    /// </summary>
    /// <param name="identifier">The identifier to ubind.</param>
    /// <param name="type">The type to unbind the identifier to.</param>
    /// <typeparam name="TIdentifier">The <see cref="Type"/> of the <see cref="IIdentifier{TId}"/>.</typeparam>
    /// <typeparam name="TId">The type of the globally unique id of the identifier.</typeparam>
    void UnbindIdentifierToType<TIdentifier, TId>(TIdentifier identifier, Type type)
        where TIdentifier : IIdentifier<TId>
        where TId : ConceptAs<Guid>;

    /// <summary>
    /// Adds a binding between an identifier and a processor builder.
    /// </summary>
    /// <param name="identifier">The identifier to bind.</param>
    /// <param name="builder">The processor builder to bind the identifier to.</param>
    /// <typeparam name="TBuilder">The <see cref="Type"/>of the processor builder.</typeparam>
    void BindIdentifierToProcessorBuilder<TBuilder>(IIdentifier identifier, TBuilder builder)
        where TBuilder : class, IEquatable<TBuilder>;
    
    /// <summary>
    /// Removes a binding between an identifier and a processor builder.
    /// </summary>
    /// <param name="identifier">The identifier to unbind.</param>
    /// <param name="builder">The processor builder to unbind the identifier to.</param>
    /// <typeparam name="TBuilder">The <see cref="Type"/>of the processor builder.</typeparam>
    void UnbindIdentifierToProcessorBuilder<TBuilder>(IIdentifier identifier, TBuilder builder)
        where TBuilder : class, IEquatable<TBuilder>;

    /// <summary>
    /// Builds a valid Dolittle application model from the bindings.
    /// </summary>
    /// <param name="buildResults">The <see cref="IClientBuildResults"/> for keeping track of build results.</param>
    /// <returns>The valid <see cref="IModel"/> representing the Dolittle application model.</returns>
    IModel Build(IClientBuildResults buildResults);

}
