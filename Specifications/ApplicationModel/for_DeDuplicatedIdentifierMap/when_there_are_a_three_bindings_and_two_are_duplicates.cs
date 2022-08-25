// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.SDK.ApplicationModel.for_DeDuplicatedIdentifierMap;

public class when_there_are_a_three_bindings_and_two_are_duplicates
{
    class duplicated_binding_value {}
    class unique_binding_value {}
    
    static given.an_identifier key;
    static TypeBinding<given.an_identifier, given.an_id> duplicate_binding;
    static TypeBinding<given.an_identifier, given.an_id> unique_binding;
    static IdentifierMap<Type> map;
    static DeDuplicatedIdentifierMap<Type> result;

    Establish context = () =>
    {
        key = new given.an_identifier(new given.an_id(Guid.Parse("bce9893a-0da7-4f96-8dcb-e1233f11e771")));
        map = new IdentifierMap<Type>();
        duplicate_binding = new TypeBinding<given.an_identifier, given.an_id>(key, typeof(duplicated_binding_value));
        duplicate_binding = new TypeBinding<given.an_identifier, given.an_id>(key, typeof(duplicated_binding_value));
        unique_binding = new TypeBinding<given.an_identifier, given.an_id>(key, typeof(unique_binding_value));
        map.AddBinding(duplicate_binding, duplicate_binding.Type);
        map.AddBinding(duplicate_binding, duplicate_binding.Type);
        map.AddBinding(unique_binding, unique_binding.Type);
    };

    Because of = () => result = new DeDuplicatedIdentifierMap<Type>(map);

    It should_have_the_correct_keys = () => result.Keys.ShouldContainOnly(key.Id.Value);
    It should_have_the_correct_binding = () => result[key.Id.Value].ShouldContainOnly(new IdentifierMapBinding<Type>(duplicate_binding, duplicate_binding.Type), new IdentifierMapBinding<Type>(unique_binding, unique_binding.Type));
}
