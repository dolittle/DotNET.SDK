// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Concepts;

namespace Basic
{
    class MyString : ConceptAs<string>
    {
        public MyString(string myString) => Value = myString;

        public static implicit operator MyString(string myString) => new MyString(myString);
    }
}
