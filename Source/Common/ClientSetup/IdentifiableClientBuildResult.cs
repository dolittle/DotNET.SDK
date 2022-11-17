// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Common.Model;

namespace Dolittle.SDK.Common.ClientSetup;

/// <summary>
/// Represents a <see cref="ClientBuildResult"/> for a specific identifiable thing.
/// </summary>
/// <param name="Identifier">The <see cref="IIdentifier"/>.</param>
/// <param name="Result">The <see cref="ClientBuildResult"/>.</param>
public record IdentifiableClientBuildResult(IIdentifier Identifier, ClientBuildResult Result);
