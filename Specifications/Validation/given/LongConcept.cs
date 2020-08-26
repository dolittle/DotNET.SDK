// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Concepts;

namespace Dolittle.Validation.Concepts.given
{
    public class LongConcept : ConceptAs<long>
    {
        public static implicit operator LongConcept(long value)
        {
            return new LongConcept { Value = value };
        }
    }
}