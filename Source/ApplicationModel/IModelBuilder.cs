// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Concepts;

namespace Dolittle.SDK.ApplicationModel;

/// <summary>
/// Defines a builder for building an <see cref="IApplicationModel"/>.
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

}
