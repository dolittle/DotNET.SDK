// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Concepts;

namespace Dolittle.SDK.Common.Model;

/// <summary>
/// Represents an implementation of <see cref="IModelBuilder"/>.
/// </summary>
public class ModelBuilder : IModelBuilder
{
    readonly IdentifierMap<Type> _typesByIdentifier = new();
    readonly IdentifierMap<object> _processorBuildersByIdentifier = new();

    /// <inheritdoc />
    public void BindIdentifierToType<TIdentifier, TId>(TIdentifier identifier, Type type)
        where TIdentifier : IIdentifier<TId>
        where TId : ConceptAs<Guid>
        => _typesByIdentifier.AddBinding(new TypeBinding<TIdentifier, TId>(identifier, type), type);

    /// <inheritdoc />
    public void UnbindIdentifierToType<TIdentifier, TId>(TIdentifier identifier, Type type)
        where TIdentifier : IIdentifier<TId>
        where TId : ConceptAs<Guid>
    {
        if (!Unbind(_typesByIdentifier, identifier, type))
        {
            throw new CannotUnbindIdentifierFromTypeThatIsNotBound(identifier, type);
        }
    }

    /// <inheritdoc />
    public void BindIdentifierToProcessorBuilder<TBuilder>(IIdentifier identifier, TBuilder builder)
        where TBuilder : class, IEquatable<TBuilder>
        => _processorBuildersByIdentifier.AddBinding(new ProcessorBuilderBinding<TBuilder>(identifier, builder), builder);

    /// <inheritdoc />
    public void UnbindIdentifierToProcessorBuilder<TBuilder>(IIdentifier identifier, TBuilder builder)
        where TBuilder : class, IEquatable<TBuilder>
    {
        if (!Unbind(_processorBuildersByIdentifier, identifier, builder))
        {
            throw new CannotUnbindIdentifierFromProcessorBuilderThatIsNotBound(identifier, builder);
        }
    }

    /// <summary>
    /// Builds a valid Dolittle application model from the bindings.
    /// </summary>
    /// <param name="buildResults">The <see cref="IClientBuildResults"/> for keeping track of build results.</param>
    /// <returns>The valid <see cref="IModel"/> representing the Dolittle application model.</returns>
    public IModel Build(IClientBuildResults buildResults)
    {
        var validBindings = new List<IBinding>();
        var deDuplicatedTypes = new DeDuplicatedIdentifierMap<Type>(
            _typesByIdentifier,
            (binding, _, numDuplicates) =>
            {
                buildResults.AddInformation($"{binding} appeared {numDuplicates} times");
            });
        var deDuplicatedProcessorBuilders = new DeDuplicatedIdentifierMap<object>(
            _processorBuildersByIdentifier,
            (binding, _, numDuplicates) =>
            {
                buildResults.AddInformation($"{binding} appeared {numDuplicates} times");
            });

        var singlyBoundTypes = new SinglyBoundDeDuplicatedIdentifierMap<Type>(
            deDuplicatedTypes,
            (type, identifiers) =>
            {
                var sb = new StringBuilder();
                sb.Append(FormattableString.Invariant($"Type {type} is bound to multiple identifiers:"));
                foreach (var identifier in identifiers)
                {
                    sb.Append(FormattableString.Invariant($"\n\t{identifier}. This binding will be ignored"));
                }
                buildResults.AddFailure(sb.ToString());
            });
        var singlyBoundProcessorBuilders = new SinglyBoundDeDuplicatedIdentifierMap<object>(
            deDuplicatedProcessorBuilders,
            (processorBuilder, identifiers) =>
            {
                var sb = new StringBuilder();
                sb.Append(FormattableString.Invariant($"Processor Builder {processorBuilder} is bound to multiple identifiers:"));
                foreach (var identifier in identifiers)
                {
                    sb.Append(FormattableString.Invariant($"\n\t{identifier}. This binding will be ignored"));
                }
                buildResults.AddFailure(sb.ToString());
            });
        var ids = singlyBoundTypes.Select(_ => _.Key).Concat(singlyBoundProcessorBuilders.Select(_ => _.Key)).ToHashSet();

        foreach (var id in ids)
        {
            var (coexistentTypes, conflictingTypes) = SplitCoexistingAndConflictingBindings(singlyBoundTypes, id);
            var (coexistentProcessorBuilders, conflictingProcessorBuilders) = SplitCoexistingAndConflictingBindings(singlyBoundProcessorBuilders, id);

            if (!conflictingTypes.Any() && !conflictingProcessorBuilders.Any())
            {
                validBindings.AddRange(coexistentTypes.Select(_ => _.Binding).Concat(coexistentProcessorBuilders.Select(_ => _.Binding)));
                continue;
            }
            AddFailedBuildResultsForConflictingBindings(id, conflictingTypes, conflictingProcessorBuilders, buildResults);
            if (coexistentTypes.Any() || coexistentProcessorBuilders.Any())
            {
                AddFailedBuildResultsForCoexistentBindings(id, coexistentTypes, coexistentProcessorBuilders, buildResults);
            }
        }

        foreach (var binding in validBindings)
        {
            buildResults.AddInformation($"{binding} will be bound");
        }
        return new Model(validBindings);
    }

    static void AddFailedBuildResultsForConflictingBindings(
        Guid id,
        IdentifierMapBinding<Type>[] conflictingTypes,
        IdentifierMapBinding<object>[] conflictingProcessorBuilders,
        IClientBuildResults buildResults)
    {
        var conflicts = new List<string>();
        if (conflictingTypes.Any())
        {
            conflicts.Add("types");
        }
        if (conflictingProcessorBuilders.Any())
        {
            conflicts.Add("processors");
        }
        var sb = new StringBuilder();
        sb.Append(FormattableString.Invariant($"The identifier {id} was bound to conflicting {string.Join(" and ", conflicts)}:"));
        foreach (var (binding, type) in conflictingTypes)
        {
            sb.Append(FormattableString.Invariant($"\n\t{binding.Identifier} was bound to type {type.Name}. This binding will be ignored"));
            buildResults.AddFailure(binding.Identifier, binding.Identifier.Alias, "Will be ignored because it is bound to multiple types");
        }
        foreach (var (binding, processorBuilder) in conflictingProcessorBuilders)
        {
            sb.Append(FormattableString.Invariant($"\n\t{binding.Identifier} was bound to processor builder {processorBuilder}. This binding will be ignored"));
            buildResults.AddFailure(binding.Identifier, binding.Identifier.Alias, "Will be ignored because it is bound to multiple processor builders");
        }
        buildResults.AddFailure(sb.ToString());
    }
    static void AddFailedBuildResultsForCoexistentBindings(Guid id, IEnumerable<IdentifierMapBinding<Type>> coexistentTypes, IEnumerable<IdentifierMapBinding<object>> coexistentProcessorBuilders, IClientBuildResults buildResults)
    {
        var sb = new StringBuilder();
        sb.Append(FormattableString.Invariant($"The identifier {id} was also bound to:"));
        foreach (var (binding, type) in coexistentTypes)
        {
            sb.Append(FormattableString.Invariant($"\n\t{binding.Identifier} binding to type {type.Name}. This binding will be ignored"));
        }
        foreach (var (binding, processorBuilder) in coexistentProcessorBuilders)
        {
            sb.Append(FormattableString.Invariant($"\n\t{binding.Identifier} binding to processor builder {processorBuilder}. This binding will be ignored"));
        }
        buildResults.AddFailure(sb.ToString());
    }


    static (IdentifierMapBinding<TValue>[] coexisting, IdentifierMapBinding<TValue>[] conflicting) SplitCoexistingAndConflictingBindings<TValue>(SinglyBoundDeDuplicatedIdentifierMap<TValue> map, Guid key)
    {
        if (!map.TryGetValue(key, out var bindings))
        {
            return (Enumerable.Empty<IdentifierMapBinding<TValue>>().ToArray(), Enumerable.Empty<IdentifierMapBinding<TValue>>().ToArray());
        }
        var conflicts = new HashSet<IIdentifier>();
        foreach (var (binding, bindingValue) in bindings)
        {
            foreach (var (otherBinding, _) in from otherBinding in bindings
                                           let canCoexist = (binding.Identifier.Equals(otherBinding.Binding.Identifier) && bindingValue.Equals(otherBinding.BindingValue))
                                            || (binding.Identifier.CanCoexistWith(otherBinding.Binding.Identifier) && !bindingValue.Equals(otherBinding.BindingValue))
                                           where !canCoexist select otherBinding)
            {
                conflicts.Add(binding.Identifier);
                conflicts.Add(otherBinding.Identifier);
            }
        }
        var coexisting = bindings.Where(_ => !conflicts.Contains(_.Binding.Identifier));
        var conflicting = bindings.Where(_ => conflicts.Contains(_.Binding.Identifier));
        return (coexisting.ToArray(), conflicting.ToArray());
    }

    static bool Unbind<TValue>(IdentifierMap<TValue> map, IIdentifier identifier, TValue value)
    {
        if (!map.TryGetValue(identifier.Id, out var bindings))
        {
            return false;
        }
        var numRemoved = bindings.RemoveAll(mappedBinding => identifier.Equals(mappedBinding.Binding.Identifier) && value.Equals(mappedBinding.BindingValue));
        map[identifier.Id] = bindings;
        return numRemoved > 0;
    }
}
