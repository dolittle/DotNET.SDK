// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.SDK.Concepts;

namespace Dolittle.SDK.ApplicationModel;

/// <summary>
/// Represents an identifier for a part of the <see cref="ApplicationModel"/>.
/// </summary>
/// <typeparam name="TId">The <see cref="Type"/> of the unique identifier.</typeparam>
/// <typeparam name="TExtras">The <see cref="Type"/> of the extra properties that should be used to check if similar identifiers can coexist.</typeparam>
public abstract class Identifier<TId, TExtras> : Identifier<TId>
    where TId : ConceptAs<Guid>
    where TExtras : IEquatable<TExtras>
{
    readonly TExtras _extras;

    /// <summary>
    /// Initializes an instance of the <see cref="Identifier{TId,TExtras}"/> class.
    /// </summary>
    /// <param name="tag">The tag name of this identifier.</param>
    /// <param name="id">The globally unique id for the identifier.</param>
    /// <param name="alias">The identifier alias.</param>
    /// <param name="extras">The extra data for the identifier.</param>
    protected Identifier(string tag, TId id, TExtras extras, IdentifierAlias? alias)
        : base(tag, id, alias)
    {
        _extras = extras;
    }
    
    /// <summary>
    /// Initializes an instance of the <see cref="Identifier{TId,TExtras}"/> class.
    /// </summary>
    /// <param name="tag">The tag name of this identifier.</param>
    /// <param name="id">The globally unique id for the identifier.</param>
    /// <param name="alias">The identifier alias.</param>
    /// <param name="extras">The extra data for the identifier.</param>
    protected Identifier(TId id, TExtras extras, IdentifierAlias? alias)
        : this("", id, extras, alias)
    {
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return base.Equals(obj)
            && obj is Identifier<TId, TExtras> other
            && _extras.Equals(other._extras);
    }

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(Id, _extras);
    
    /// <inheritdoc />
    public override string ToString() => $"{Title}({Id.Value} {ExtrasAsString()})";

    string ExtrasAsString()
        => _extras == null ? string.Empty : $"{typeof(TExtras).Name}: {_extras}";
}

/// <summary>
/// Represents an identifier for a part of the <see cref="ApplicationModel"/>.
/// </summary>
/// <typeparam name="TId">The <see cref="Type"/> of the unique identifier.</typeparam>
public abstract class Identifier<TId> : IIdentifier<TId>
    where TId : ConceptAs<Guid>
{
    /// <summary>
    /// Initializes an instance of the <see cref="Identifier{TId}"/> class.
    /// </summary>
    /// <param name="tag">The tag name of this identifier.</param>
    /// <param name="id">The globally unique id for the identifier.</param>
    /// <param name="alias">The identifier alias.</param>
    protected Identifier(string tag, TId id, IdentifierAlias? alias)
    {
        Id = id ?? throw new IdentifierIdCannotBeNull(GetType(), typeof(TId));
        Tag = tag ?? "";
        Alias = alias ?? "";
    }

    /// <inheritdoc />
    public TId Id { get; }

    /// <inheritdoc />
    public IdentifierAlias Alias { get; }

    /// <inheritdoc />
    public bool HasAlias => Alias?.Exists ?? false;

    /// <inheritdoc />
    Guid IIdentifier.Id => Id.Value;
    
    /// <summary>
    /// Gets the tag name.
    /// </summary>
    protected string Tag { get; init; }

    /// <inheritdoc />
    public virtual bool CanCoexistWith(IIdentifier<ConceptAs<Guid>> identifier) => false;

    /// <inheritdoc />
    public bool CanCoexistWith(IIdentifier identifier) => identifier is IIdentifier<TId> typedIdentifier && CanCoexistWith(typedIdentifier);

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        if (obj is null)
        {
            return false;
        }
        if (ReferenceEquals(this, obj))
        {
            return true;
        }
        return obj.GetType() == GetType() && Id.Equals(((Identifier<TId>)obj).Id);
    }

    /// <inheritdoc />
    public override int GetHashCode() => EqualityComparer<TId>.Default.GetHashCode(Id);

    /// <inheritdoc />
    public override string ToString() => $"{Title}({Id.Value})";

    /// <summary>
    /// Gets the beginning part used in the <see cref="ToString"/> method.
    /// </summary>
    protected string Title => Alias.Exists ? Tag : $"{Tag} {Alias}";
}
