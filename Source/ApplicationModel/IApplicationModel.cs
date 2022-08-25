// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.SDK.Concepts;

namespace Dolittle.SDK.ApplicationModel;

/// <summary>
/// Defines a Dolittle application model.
/// </summary>
public interface IApplicationModel
{
    /// <summary>
    /// Gets all bindings for the given identifier.
    /// </summary>
    IEnumerable<IBinding<TIdentifier, TId>> GetBindings<TIdentifier, TId>()
        where TIdentifier : IIdentifier<TId>
        where TId : ConceptAs<Guid>;

    /// <summary>
    /// Gets the <see cref="TypeBinding{TIdentifier,TId}"/> bindings for a specific kind of identifier to types. 
    /// </summary>
    /// <typeparam name="TIdentifier">The <see cref="Type"/> of the <see cref="IIdentifier{TId}"/>.</typeparam>
    /// <typeparam name="TId">The type of the globally unique id of the identifier.</typeparam>
    /// <returns>The bound types.</returns>
    IEnumerable<TypeBinding<TIdentifier, TId>> GetTypeBindings<TIdentifier, TId>()
        where TIdentifier : IIdentifier<TId>
        where TId : ConceptAs<Guid>;

    /// <summary>
    /// Gets the bindings for a specific kind of processor builder.
    /// </summary>
    /// <typeparam name="TProcessor">The <see cref="Type"/> of the processor to build.</typeparam>
    /// <typeparam name="TBuilder">The <see cref="Type"/>of the processor builder.</typeparam>
    /// <typeparam name="TIdentifier">The <see cref="Type"/> of the <see cref="IIdentifier{TId}"/>.</typeparam>
    /// <typeparam name="TId">The type of the globally unique id of the identifier.</typeparam>
    /// <returns>The bound processor builders.</returns>
    IEnumerable<ProcessorBuilderBinding<TProcessor, TBuilder, TIdentifier, TId>> GetProcessorBuilderBindings<TProcessor, TBuilder, TIdentifier, TId>()
        where TProcessor : class
        where TBuilder : IProcessorBuilder<TProcessor, TIdentifier, TId>
        where TIdentifier : IIdentifier<TId>
        where TId : ConceptAs<Guid>;
}
