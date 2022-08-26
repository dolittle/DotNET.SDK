// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Dolittle.SDK.ApplicationModel.ClientSetup;
using Dolittle.SDK.Common;
using Dolittle.SDK.Concepts;

namespace Dolittle.SDK.ApplicationModel;

/// <summary>
/// Represents a thing that knows about decorated types representing a unique <typeparamref name="TIdentifier"/> and how to add it to the <see cref="IApplicationModel"/>.
/// </summary>
/// <typeparam name="TDecorator">The <see cref="Type"/> of the class decorator.</typeparam>
/// <typeparam name="TIdentifier">The <see cref="Type"/> of the <see cref="IIdentifier{TId}"/>.</typeparam>
/// <typeparam name="TId">The type of the globally unique id of the identifier.</typeparam>
public class DecoratedTypeBindingsToModelAdder<TDecorator, TIdentifier, TId>
    where TDecorator : Attribute, IDecoratedTypeDecorator<TIdentifier>
    where TIdentifier : IIdentifier<TId>
    where TId : ConceptAs<Guid>
{
    readonly string _decoratedTypeTag;
    readonly IModelBuilder _modelBuilder;
    readonly IClientBuildResults _buildResults;

    /// <summary>
    /// Initializes an instance of the <see cref="DecoratedTypeBindingsToModelAdder{TDecorator, TIdentifier,TId}"/> class.
    /// </summary>
    /// <param name="decoratedTypeTag">The log-friendly name of the models decorated with the <typeparamref name="TDecorator"/>.</param>
    /// <param name="modelBuilder">The <see cref="IModelBuilder"/> to bind the derived <see cref="TypeBinding{TIdentifier,TId}"/> on.</param>
    /// <param name="buildResults">The <see cref="IClientBuildResults"/> for adding <see cref="FailureBuildResult"/> when the <typeparamref name="TDecorator"/> is not present on a given <see cref="Type"/>.</param>
    public DecoratedTypeBindingsToModelAdder(string decoratedTypeTag, IModelBuilder modelBuilder, IClientBuildResults buildResults)
    {
        _decoratedTypeTag = decoratedTypeTag;
        _modelBuilder = modelBuilder;
        _buildResults = buildResults;
    }
    
    /// <summary>
    /// Try add the <typeparamref name="TIdentifier"/> derived from the decorator on the given <see cref="Type"/>.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> that is decorated with an <see cref="Attribute"/> that implements <see cref="IDecoratedTypeDecorator{TIdentifier}"/> and is to be bound to the derived <typeparamref name="TIdentifier"/>.</param>
    /// <param name="binding">The <typeparamref name="TIdentifier"/> derived from the decoration.</param>
    /// <returns>A value indicating whether the given <see cref="Type"/> is decorated with <typeparamref name="TDecorator"/>.</returns>
    public bool TryAdd(Type type, [NotNullWhen(true)]out TypeBinding<TIdentifier, TId>? binding)
    {
        binding = null;
        if (!type.TryGetDecorator<TDecorator>(out var decorator))
        {
            _buildResults.AddFailure($"The {_decoratedTypeTag} class {type.Name} is is not decorated as an {_decoratedTypeTag}", $"Add the [{nameof(TDecorator)}] decorator to the class");
            return false;
        }
        binding = new TypeBinding<TIdentifier, TId>(decorator.GetIdentifier(type), type);
        AddBinding(binding.Identifier, type);
        return true;
    }

    /// <summary>
    /// Adds all the <typeparamref name="TIdentifier"/> bindings found in the <see cref="Assembly"/> <see cref="Assembly.ExportedTypes"/>;
    /// </summary>
    /// <param name="assembly">The <see cref="Assembly"/> to search in.</param>
    /// <returns>The </returns>
    public IEnumerable<TypeBinding<TIdentifier, TId>> AddFromAssembly(Assembly assembly)
    {
        var result = new List<TypeBinding<TIdentifier, TId>>();
        try
        {
            foreach (var type in assembly.ExportedTypes)
            {
                if (!type.TryGetIdentifier<TIdentifier>(out var identifier))
                {
                    continue;
                }
                AddBinding(identifier, type);
                result.Add(new TypeBinding<TIdentifier, TId>(identifier, type));
            }
        }
        catch
        {
        }
        return result;
    }

    void AddBinding(TIdentifier identifier, Type type)
    {
        _modelBuilder.BindIdentifierToType<TIdentifier, TId>(identifier, type);
    }
}
