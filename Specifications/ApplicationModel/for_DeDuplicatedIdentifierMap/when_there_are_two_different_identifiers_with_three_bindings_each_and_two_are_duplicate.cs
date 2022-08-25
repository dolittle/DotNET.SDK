// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.SDK.ApplicationModel.for_DeDuplicatedIdentifierMap;

public class when_there_are_two_different_identifiers_with_three_bindings_each_and_two_are_duplicate
{
    class duplicated_binding_value {}
    class unique_binding_value {}
    
    static given.an_identifier first_key;
    static given.an_identifier second_key;
    static TypeBinding<given.an_identifier, given.an_id> duplicate_binding_for_first_key;
    static TypeBinding<given.an_identifier, given.an_id> unique_binding_for_first_key;
    static TypeBinding<given.an_identifier, given.an_id> duplicate_binding_for_second_key;
    static TypeBinding<given.an_identifier, given.an_id> unique_binding_for_second_key;
    static IdentifierMap<Type> map;
    static DeDuplicatedIdentifierMap<Type> result;

    Establish context = () =>
    {
        first_key = new given.an_identifier(new given.an_id(Guid.Parse("9ca35804-37d9-4b55-877a-90b2ee7c5b2e")));
        second_key = new given.an_identifier(new given.an_id(Guid.Parse("e5f9f111-d37a-4e70-9a89-4633e3d4f248")));
        map = new IdentifierMap<Type>();
        duplicate_binding_for_first_key = new TypeBinding<given.an_identifier, given.an_id>(first_key, typeof(duplicated_binding_value));
        duplicate_binding_for_first_key = new TypeBinding<given.an_identifier, given.an_id>(first_key, typeof(duplicated_binding_value));
        unique_binding_for_first_key = new TypeBinding<given.an_identifier, given.an_id>(first_key, typeof(unique_binding_value));
        
        duplicate_binding_for_second_key = new TypeBinding<given.an_identifier, given.an_id>(second_key, typeof(duplicated_binding_value));
        duplicate_binding_for_second_key = new TypeBinding<given.an_identifier, given.an_id>(second_key, typeof(duplicated_binding_value));
        unique_binding_for_second_key = new TypeBinding<given.an_identifier, given.an_id>(second_key, typeof(unique_binding_value));
        
        map.AddBinding(duplicate_binding_for_first_key, duplicate_binding_for_first_key.Type);
        map.AddBinding(duplicate_binding_for_first_key, duplicate_binding_for_first_key.Type);
        map.AddBinding(unique_binding_for_first_key, unique_binding_for_first_key.Type);
        
        map.AddBinding(duplicate_binding_for_second_key, duplicate_binding_for_second_key.Type);
        map.AddBinding(duplicate_binding_for_second_key, duplicate_binding_for_second_key.Type);
        map.AddBinding(unique_binding_for_second_key, unique_binding_for_second_key.Type);
    };

    Because of = () => result = new DeDuplicatedIdentifierMap<Type>(map);

    It should_have_the_correct_keys = () => result.Keys.ShouldContainOnly(first_key.Id.Value, second_key.Id.Value);
    It should_have_the_correct_bindings_for_the_first_key = () => result[first_key.Id.Value].ShouldContainOnly(
        new IdentifierMapBinding<Type>(duplicate_binding_for_first_key, duplicate_binding_for_first_key.Type),
        new IdentifierMapBinding<Type>(unique_binding_for_first_key, unique_binding_for_first_key.Type));

    It should_have_the_correct_bindings_for_the_second_key = () => result[second_key.Id.Value].ShouldContainOnly(
        new IdentifierMapBinding<Type>(duplicate_binding_for_second_key, duplicate_binding_for_second_key.Type),
        new IdentifierMapBinding<Type>(unique_binding_for_second_key, unique_binding_for_second_key.Type));
}
