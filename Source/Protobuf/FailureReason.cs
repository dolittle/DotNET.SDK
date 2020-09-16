// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Concepts;

namespace Dolittle.SDK.Protobuf
{
    /// <summary>
    /// Represents the reason for failure.
    /// </summary>
    public class FailureReason : ConceptAs<string>
    {
        /// <summary>
        /// Implicitly converts <see cref="string" /> to <see cref="FailureReason" />.
        /// </summary>
        /// <param name="failureReason"><see cref="string" /> to convert.</param>
        public static implicit operator FailureReason(string failureReason) => new FailureReason { Value = failureReason };
    }
}
