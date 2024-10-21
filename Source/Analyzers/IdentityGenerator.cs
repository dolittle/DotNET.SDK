// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Analyzers;

/// <summary>
/// Hook to make code fixes testable
/// </summary>
static class IdentityGenerator
{
    static string? _overrideRedaction;
    internal static string? Override { get; set; }

    internal static string? OverrideRedaction
    {
        get => _overrideRedaction;
        set
        {
            if(value is not null && !value.StartsWith(DolittleConstants.Identifiers.RedactionIdentityPrefix))
                throw new ArgumentException("Redaction identity must start with 'redaction-'");
            _overrideRedaction = value;
        }
    }

    public static string Generate() => Override ?? Guid.NewGuid().ToString();
    
    public static string GenerateRedactionId() => OverrideRedaction ?? DolittleConstants.Identifiers.RedactionIdentityPrefix + Guid.NewGuid().ToString().Substring(DolittleConstants.Identifiers.RedactionIdentityPrefix.Length);
}
