// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Concepts.given;

namespace Dolittle.SDK.Concepts.for_ConceptAs.given
{
    public class test_concepts
    {
        protected static IntConcept least = 0;
        protected static IntConcept most = 10;
        protected static IntConcept another_instance_of_most = most.Value;
        protected static IntConcept middle = 5;
    }
}