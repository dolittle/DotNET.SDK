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
    readonly IEnumerable<IBinding> _bindings;

    /// <summary>
    /// Initializes an instance of the <see cref="ApplicationModel"/> class.
    /// </summary>
    /// <param name="bindings">The <see cref="IBinding"/> bindings that builds the Dolittle application model.</param>
    public ApplicationModel(IEnumerable<IBinding> bindings)
    {
        _bindings = bindings;
    }

    /// <inheritdoc />
    public IEnumerable<IBinding<TIdentifier, TId>> GetBindings<TIdentifier, TId>()
        where TIdentifier : IIdentifier<TId>
        where TId : ConceptAs<Guid>
        => BindingsOfType<IBinding<TIdentifier, TId>>();

    /// <inheritdoc />
    public IEnumerable<TypeBinding<TIdentifier, TId>> GetTypeBindings<TIdentifier, TId>()
        where TIdentifier : IIdentifier<TId>
        where TId : ConceptAs<Guid>
        => BindingsOfType<TypeBinding<TIdentifier, TId>>();

    /// <inheritdoc />
    public IEnumerable<ProcessorBuilderBinding<TProcessor, TBuilder, TIdentifier, TId>> GetProcessorBuilderBindings<TProcessor, TBuilder, TIdentifier, TId>()
        where TProcessor : class
        where TBuilder : IProcessorBuilder<TProcessor, TIdentifier, TId>
        where TIdentifier : IIdentifier<TId>
        where TId : ConceptAs<Guid>
        => BindingsOfType<ProcessorBuilderBinding<TProcessor, TBuilder, TIdentifier, TId>>();

    IEnumerable<TBinding> BindingsOfType<TBinding>()
        where TBinding : IBinding
    {
        var bindings = new List<TBinding>();
        foreach (var binding in _bindings)
        {
            if (binding is TBinding specificBinding)
            {
                bindings.Add(specificBinding);
            }
        }
        return bindings;
    }
}
