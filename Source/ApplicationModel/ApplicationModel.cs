// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.SDK.Concepts;

namespace Dolittle.SDK.ApplicationModel;

/// <summary>
/// Represents an implementation of <see cref="IApplicationModel"/>.
/// </summary>
public class ApplicationModel : IApplicationModel
{
    /// <summary>
    /// Initializes an instance of the <see cref="ApplicationModel"/> class.
    /// </summary>
    /// <param name="bindings">The <see cref="IBinding"/> bindings that builds the Dolittle application model.</param>
    public ApplicationModel(IEnumerable<IBinding> bindings)
    {
        Bindings = bindings;
    }

    /// <inheritdoc />
    public IEnumerable<IBinding> Bindings { get; }

    /// <inheritdoc />
    public IEnumerable<TypeBinding<TIdentifier, TId>> GetTypeBindings<TIdentifier, TId>()
        where TIdentifier : IIdentifier<TId>
        where TId : ConceptAs<Guid>
        => BindingsOfType<TypeBinding<TIdentifier, TId>>();

    /// <inheritdoc />
    public IEnumerable<ProcessorBuilderBinding<TBuilder>> GetProcessorBuilderBindings<TBuilder>()
        where TBuilder : class, IEquatable<TBuilder>
        => BindingsOfType<ProcessorBuilderBinding<TBuilder>>();

    IEnumerable<TBinding> BindingsOfType<TBinding>()
        where TBinding : IBinding
    {
        var bindings = new List<TBinding>();
        foreach (var binding in Bindings)
        {
            if (binding is TBinding specificBinding)
            {
                bindings.Add(specificBinding);
            }
        }
        return bindings;
    }
}
