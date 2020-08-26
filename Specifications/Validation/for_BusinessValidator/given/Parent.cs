// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Validation.for_BusinessValidator.given
{
    public class Parent
    {
        public string SimpleStringProperty { get; set; }

        public int SimpleIntegerProperty { get; set; }

        public ConceptAsLong Id { get; set; }

        public Child Child { get; set; }
    }
}