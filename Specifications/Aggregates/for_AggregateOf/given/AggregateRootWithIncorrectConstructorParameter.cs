// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.SDK.Aggregates.for_AggregateOf.given
{
    [AggregateRoot("3ee85a6e-bf0c-4235-9a92-a9bb2a0ffd12")]
    public class AggregateRootWithIncorrectConstructorParameter : AggregateRoot
    {
        public AggregateRootWithIncorrectConstructorParameter(string some_string)
            : base("83c67dd2-56d1-4fa1-8e2d-5a121724c58b")
        {
        }
    }
}