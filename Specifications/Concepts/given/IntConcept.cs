// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.SDK.Concepts.given
{
    public record IntConcept(int Value) : ConceptAs<int>(Value)
    {
        public IntConcept()
        {
        }

        public IntConcept(int value)
        {
            Value = value;
        }

        public static implicit operator IntConcept(int value)
        {
            return new(value);
        }
    }
}
