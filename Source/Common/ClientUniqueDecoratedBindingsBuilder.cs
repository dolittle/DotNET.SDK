// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Common.ClientSetup;

namespace Dolittle.SDK.Common;

/// <summary>
/// Represents an implementation of <see cref="ICanBuildUniqueDecoratedBindings{TIdentifier,TValue}"/>.
/// </summary>
/// <typeparam name="TIdentifier">The <see cref="Type" /> of the unique identifier.</typeparam>
/// <typeparam name="TValue">The <see cref="Type" /> of the value to associate with the unique identifier.</typeparam>
/// <typeparam name="TDecorator">The <see cref="Type"/> of the <see cref="Attribute"/> used to decorate the <see cref="Type"/> with the <typeparamref name="TIdentifier"/>.</typeparam>
public class ClientUniqueDecoratedBindingsBuilder<TIdentifier, TValue, TDecorator> : ClientUniqueBindingsBuilder<TIdentifier, TValue>, ICanBuildUniqueDecoratedBindings<TIdentifier, TValue>
    where TIdentifier : IEquatable<TIdentifier>
    where TValue : class
    where TDecorator : Attribute, IUniqueBindingDecorator<TIdentifier>
{
    const string AttributeString = $"[{nameof(TDecorator)}(...)]";
    
    /// <summary>
    /// Initializes an instance of the <see cref="ClientUniqueDecoratedBindingsBuilder{TIdentifier,TValue,TDecorator}"/> class.
    /// </summary>
    /// <param name="identifierLabel">The label of the identifier. Used for <see cref="IClientBuildResults"/>.</param>
    /// <param name="valueLabel">The label of the value. Used for <see cref="IClientBuildResults"/>.</param>
    public ClientUniqueDecoratedBindingsBuilder(string identifierLabel = nameof(TIdentifier), string valueLabel = nameof(TValue))
        : base(identifierLabel, valueLabel)
    {
    }
    
    /// <inheritdoc />
    public override void Add(TIdentifier identifier, TValue value)
    {
        if (TryGetIdentifierFromDecoratedType(value, out var decoratedIdentifier)
            && !identifier.Equals(decoratedIdentifier))
        {
            AddBuildResult(ClientBuildResult.Failure(
                $"Trying to associate {ValueLabel} {value} with {IdentifierLabel} {identifier}, but it is already associated to {decoratedIdentifier}",
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
    /// Try to get the <typeparamref name="TDecorator"/> from the <typeparamref name="TValue"/>. 
    /// </summary>
    /// <param name="value">The <typeparamref name="TValue"/> to get the <typeparamref name="TDecorator"/> from.</param>
    /// <param name="decorator">The outputted <typeparamref name="TDecorator"/>.</param>
    /// <returns>The value indicating whether the <typeparamref name="TDecorator"/> could be extracted from the <typeparamref name="TValue"/>.</returns>
    protected bool TryGetDecorator(TValue value, out TDecorator decorator)
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
        if (!TryGetDecorator(value, out var attribute))
        {
            return false;
        }
        identifier = attribute.GetIdentifier();
        return true;
    }
}
