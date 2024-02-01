// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Testing.Aggregates;

namespace Specs.Kitchen.for_Kitchen.given;

public abstract class a_kitchen(): AggregateRootTests<global::Kitchen.Kitchen>(kitchen_name)
{
    const string kitchen_name = "Dolittle Tacos";
}