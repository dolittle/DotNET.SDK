// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;

namespace Dolittle.SDK.Analyzers.Suppressors;

static class SuppressionDescriptors
{
    public static readonly SuppressionDescriptor IDE0051Unused = new(
        "DSD0001",
        "IDE0051",
        "On-methods are used internally by the aggregate to mutate it's state");
}
