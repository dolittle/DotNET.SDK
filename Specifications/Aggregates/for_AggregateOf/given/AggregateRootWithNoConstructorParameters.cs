// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.SDK.Aggregates.for_AggregateOf.given
{
    [AggregateRoot("db7214f6-061e-4394-9dc9-cf5af05b72c1")]
    public class AggregateRootWithNoConstructorParameters : AggregateRoot
    {
        public AggregateRootWithNoConstructorParameters()
            : base("f47fd493-14f0-4b60-a0db-bee3d70cef6c")
        {
        }
    }
}