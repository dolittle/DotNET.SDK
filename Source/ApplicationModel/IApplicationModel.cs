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
    /// Gets the valid <see cref="IBinding"/> bindings.
    /// </summary>
    IEnumerable<IBinding> Bindings { get; }

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
    /// <typeparam name="TBuilder">The <see cref="Type"/>of the processor builder.</typeparam>
    /// <returns>The bound processor builders.</returns>
    IEnumerable<ProcessorBuilderBinding<TBuilder>> GetProcessorBuilderBindings<TBuilder>()
        where TBuilder : class, IEquatable<TBuilder>;
}
