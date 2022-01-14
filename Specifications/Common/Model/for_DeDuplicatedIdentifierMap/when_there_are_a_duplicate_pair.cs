// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.SDK.Common.Model.for_DeDuplicatedIdentifierMap;

public class when_there_are_a_duplicate_pair
{
    class some_type {}
    
    static given.an_identifier key;
    static TypeBinding<given.an_identifier, given.an_id> binding;
    static IdentifierMap<Type> map;
    static DeDuplicatedIdentifierMap<Type> result;

    Establish context = () =>
    {
        key = new given.an_identifier(new given.an_id(Guid.Parse("5c1db80b-240f-40f9-9b76-e4cb21686912")));
        map = new IdentifierMap<Type>();
        binding = new TypeBinding<given.an_identifier, given.an_id>(key, typeof(some_type));
        map.AddBinding(binding, binding.Type);
        map.AddBinding(binding, binding.Type);
    };

    Because of = () => result = new DeDuplicatedIdentifierMap<Type>(map);

    It should_have_the_correct_keys = () => result.Keys.ShouldContainOnly(key.Id.Value);
    It should_have_the_correct_binding = () => result[key.Id.Value].ShouldContainOnly(new IdentifierMapBinding<Type>(binding, binding.Type));
}
