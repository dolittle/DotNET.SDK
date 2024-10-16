// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Events.Redaction;

/// <summary>
/// Attribute to mark a property as redactable.
/// This allows the system to remove personal data from persisted events.
/// Properties that should be redactable must be nullable, and redacted properties will be set to null. 
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class RedactablePersonalDataAttribute : Attribute
{
    /// <summary>
    /// Gets the value to replace the redacted property with.
    /// </summary>
    public object? ReplacementValue { get; protected set; } = null;
}

/// <summary>
/// Attribute to mark a property as redactable.
/// This allows the system to remove personal data from persisted events.
/// This subclass allows for a specific replacement value to be set, instead of the complete removal of the property.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class RedactablePersonalDataAttribute<T> : RedactablePersonalDataAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RedactablePersonalDataAttribute{T}"/> class.
    /// </summary>
    /// <param name="replacementValue">The value to replace the redacted property with.</param>
    public RedactablePersonalDataAttribute(T replacementValue)
    {
        ReplacementValue = replacementValue;
    }
}
