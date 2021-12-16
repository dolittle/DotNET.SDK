// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.SDK.Failures;

/// <summary>
/// Represents a failure.
/// </summary>
/// <param name="Id"><see cref="FailureId" />.</param>
/// <param name="Reason"><see cref="FailureReason" />.</param>
public record Failure(FailureId Id, FailureReason Reason);
