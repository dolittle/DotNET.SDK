// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.SDK.Concepts.given
{
    public record LongConcept(long Value) : ConceptAs<long>(Value)
    {
        public static implicit operator LongConcept(long value)
        {
            return new(value);
        }
    }
}
