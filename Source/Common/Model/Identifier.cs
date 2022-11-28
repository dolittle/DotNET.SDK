// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.SDK.Concepts;

namespace Dolittle.SDK.Common.Model;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TId"></typeparam>
/// <typeparam name="TExtras"></typeparam>
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
    /// <param name="extras">The extra data for the identifier.</param>
    protected Identifier(string tag, TId id, TExtras extras) : base(tag, id)
    {
        _extras = extras;
    }

    protected bool Equals(Identifier<TId, TExtras> other) => base.Equals(other) && EqualityComparer<TExtras>.Default.Equals(_extras, other._extras);

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Identifier<TId, TExtras>)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (base.GetHashCode() * 397) ^ EqualityComparer<TExtras>.Default.GetHashCode(_extras);
        }
    }

    /// <inheritdoc />
    // public override int GetHashCode() => HashCode.Combine(Id, _extras);
    
    /// <inheritdoc />
    public override string ToString() => $"{Tag}({Id.Value} {ExtrasAsString()})";

    string ExtrasAsString()
        => _extras == null ? string.Empty : $"{typeof(TExtras).Name}: {_extras}";
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="TId"></typeparam>
public abstract class Identifier<TId> : IIdentifier<TId>
    where TId : ConceptAs<Guid>
{
    /// <summary>
    /// Initializes an instance of the <see cref="Identifier{TId}"/> class.
    /// </summary>
    /// <param name="tag">The tag name of this identifier.</param>
    /// <param name="id">The globally unique id for the identifier.</param>
    protected Identifier(string tag, TId id)
    {
        Tag = tag;
        Id = id;
    }

    /// <inheritdoc />
    public TId Id { get; }

    /// <inheritdoc />
    Guid IIdentifier.Id => Id.Value;
    
    /// <summary>
    /// Gets the tag name.
    /// </summary>
    protected string Tag { get; }

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
    public override string ToString() => $"{Tag}({Id.Value})";
}
