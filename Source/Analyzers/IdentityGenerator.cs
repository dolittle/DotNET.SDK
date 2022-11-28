// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Analyzers;

/// <summary>
/// Hook to make code fixes testable
/// </summary>
static class IdentityGenerator
{
    internal static string? Override { get; set; }

    public static string Generate() => Override ?? Guid.NewGuid().ToString();
}
