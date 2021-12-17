// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Common.ClientSetup;

namespace Dolittle.SDK.Common;

/// <summary>
/// Represents an implementation of <see cref="ICanBuildUniqueDecoratedBindings{TIdentifier,TValue,TUniqueBindings}"/>.
/// </summary>
/// <typeparam name="TIdentifier">The <see cref="Type" /> of the unique identifier.</typeparam>
/// <typeparam name="TValue">The <see cref="Type" /> of the value to associate with the unique identifier.</typeparam>
/// <typeparam name="TUniqueBindings">The <see cref="Type"/> of the <see cref="IUniqueBindings{TIdentifier,TValue}"/> to be built</typeparam>
/// <typeparam name="TDecorator">The <see cref="Type"/> of the <see cref="Attribute"/> used to decorate the <see cref="Type"/> with the <typeparamref name="TIdentifier"/>.</typeparam>
public abstract class ClientUniqueDecoratedBindingsBuilder<TIdentifier, TValue, TUniqueBindings, TDecorator> : ClientUniqueBindingsBuilder<TIdentifier, TValue, TUniqueBindings>, ICanBuildUniqueDecoratedBindings<TIdentifier, TValue, TUniqueBindings>
    where TIdentifier : IEquatable<TIdentifier>
    where TValue : class
    where TUniqueBindings : IUniqueBindings<TIdentifier, TValue>
    where TDecorator : Attribute
{
    const string AttributeString = $"[{nameof(TDecorator)}(...)]";

    /// <inheritdoc />
    public override void Add(TIdentifier identifier, TValue value)
    {
        if (TryGetIdentifierFromDecoratedType(value, out var decoratedIdentifier)
            && !identifier.Equals(decoratedIdentifier))
        {
            AddBuildResult(ClientBuildResult.Failure(
                $"Trying to associate {value} with {identifier}, but it is already associated to {decoratedIdentifier}",
                $"Either the {AttributeString} from {value} is wrong and remove that or the manual association of {value} to {identifier} is wrong and remove that"));
            return;
        }
        base.Add(identifier, value);
    }

    /// <inheritdoc />
    public void Add(TValue value)
    {
        if (TryGetIdentifierFromDecoratedType(value, out var identifier))
        {
            Add(identifier, value);
        }
        else
        {
            AddBuildResult(ClientBuildResult.Failure(
                $"{value} is missing the {AttributeString} attribute",
                $"Put the {AttributeString} attribute on the {value}"));
        }
    }

    /// <summary>
    /// Try to get the <typeparamref name="TIdentifier"/> from the <typeparamref name="TDecorator"/> on the decorated <see cref="Type"/>.
    /// </summary>
    /// <param name="value">The <typeparamref name="TValue"/> that the <typeparamref name="TDecorator"/> is derived from.</param>
    /// <param name="attribute">The <typeparamref name="TDecorator"/>.</param>
    /// <param name="identifier">The extracted <typeparamref name="TIdentifier"/>.</param>
    /// <returns>The value indicating whether the <typeparamref name="TIdentifier"/> could be extracted from the <typeparamref name="TDecorator"/>.</returns>
    protected abstract bool TryGetIdentifierFromDecorator(TValue value, TDecorator attribute, out TIdentifier identifier);

    /// <summary>
    /// Try to get the <typeparamref name="TDecorator"/> from the <typeparamref name="TValue"/>. 
    /// </summary>
    /// <param name="value">The <typeparamref name="TValue"/> to get the <typeparamref name="TDecorator"/> from.</param>
    /// <param name="decorator">The outputted <typeparamref name="TDecorator"/>.</param>
    /// <returns>The value indicating whether the <typeparamref name="TDecorator"/> could be extracted from the <typeparamref name="TValue"/>.</returns>
    protected virtual bool TryGetDecorator(TValue value, out TDecorator decorator)
    {
        decorator = default;
        if (value is Type valueAsType)
        {
            if (Attribute.GetCustomAttribute(valueAsType, typeof(TDecorator)) is not TDecorator decoratorOnType)
            {
                return false;
            }
            decorator = decoratorOnType;       
        }
        else
        {
            var type = value.GetType();
            if (Attribute.GetCustomAttribute(type, typeof(TDecorator)) is not TDecorator decoratorOnType)
            {
                return false;
            }
            decorator = decoratorOnType; 
        }
        return false;
    }

    bool TryGetIdentifierFromDecoratedType(TValue value, out TIdentifier identifier)
    {
        identifier = default;
        return TryGetDecorator(value, out var attribute) && TryGetIdentifierFromDecorator(value, attribute, out identifier);
    }
}
