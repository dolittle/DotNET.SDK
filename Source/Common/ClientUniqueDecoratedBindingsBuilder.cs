// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using Dolittle.SDK.Common.ClientSetup;

namespace Dolittle.SDK.Common;

/// <summary>
/// Represents an implementation of <see cref="ICanBuildUniqueDecoratedBindings{TIdentifier,TUniqueBindings}"/>.
/// </summary>
/// <typeparam name="TIdentifier">The <see cref="Type" /> of the unique identifier.</typeparam>
/// <typeparam name="TUniqueBindings">The <see cref="Type"/> of the <see cref="IUniqueBindings{TIdentifier,TValue}"/> to be built</typeparam>
/// <typeparam name="TDecorator">The <see cref="Type"/> of the <see cref="Attribute"/> used to decorate the <see cref="Type"/> with the <typeparamref name="TIdentifier"/>.</typeparam>
public abstract class ClientUniqueDecoratedBindingsBuilder<TIdentifier, TUniqueBindings, TDecorator> : ClientUniqueBindingsBuilder<TIdentifier, Type, TUniqueBindings>, ICanBuildUniqueDecoratedBindings<TIdentifier, TUniqueBindings>
    where TIdentifier : IEquatable<TIdentifier>
    where TUniqueBindings : IUniqueBindings<TIdentifier, Type>
    where TDecorator : Attribute
{
    const string AttributeString = $"[{nameof(TDecorator)}(...)]";

    /// <inheritdoc />
    public override void Add(TIdentifier identifier, Type type)
    {
        if (TryGetIdentifierFromDecoratedType(type, out var decoratedIdentifier)
            && !identifier.Equals(decoratedIdentifier))
        {
            AddBuildResult(ClientBuildResult.Failure(
                $"Trying to associate {type} with {identifier}, but it is already associated to {decoratedIdentifier}",
                $"Either the {AttributeString} on {type} is wrong and remove that or the manual association of {type} to {identifier} is wrong and remove that"));
            return;
        }
        base.Add(identifier, type);
    }

    /// <inheritdoc />
    public void Add(Type type)
    {
        if (TryGetIdentifierFromDecoratedType(type, out var identifier))
        {
            Add(identifier, type);
        }
        else
        {
            AddBuildResult(ClientBuildResult.Failure(
                $"{type} is missing the {AttributeString} attribute",
                $"Put the {AttributeString} attribute on the ${type} class"));
        }

    }

    /// <inheritdoc />
    public void AddAllFrom(Assembly assembly)
    {
        foreach (var type in assembly.ExportedTypes)
        {
            if (TryGetDecorator(type, out _))
            {
                Add(type);
            }
        }
    }
    
    /// <summary>
    /// Tries to get the <typeparamref name="TIdentifier"/> from the <typeparamref name="TDecorator"/> on the decorated <see cref="Type"/>.
    /// </summary>
    /// <param name="decoratedType">The <see cref="Type"/> that the <typeparamref name="TDecorator"/> is on.</param>
    /// <param name="attribute">The <typeparamref name="TDecorator"/>.</param>
    /// <param name="identifier">The extracted <typeparamref name="TIdentifier"/>.</param>
    /// <returns>The value indicating whether the <see cref="Type"/> is missing a valid <typeparamref name="TDecorator"/>.</returns>
    protected abstract bool TryGetIdentifierFromDecorator(Type decoratedType, TDecorator attribute, out TIdentifier identifier);

    bool TryGetIdentifierFromDecoratedType(Type type, out TIdentifier identifier)
    {
        identifier = default;
        return TryGetDecorator(type, out var attribute) && TryGetIdentifierFromDecorator(type, attribute, out identifier);
    }

    static bool TryGetDecorator(Type type, out TDecorator attribute)
    {
        attribute = default;
        if (Attribute.GetCustomAttribute(type, typeof(TDecorator)) is not TDecorator attributeOnType)
        {
            return false;
        }
        attribute = attributeOnType;
        return true;
    }
}
