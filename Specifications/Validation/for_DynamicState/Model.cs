// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;

namespace Dolittle.Validation.for_DynamicState
{
    public class Model
    {
        public static PropertyInfo TheStringProperty = typeof(Model).GetProperty("TheString");
        public bool TheStringGetCalled = false;

        public bool TheStringSetCalled = false;
        string _theString;

        public string TheString
        {
            get
            {
                TheStringGetCalled = true;
                return _theString;
            }

            set
            {
                TheStringSetCalled = true;
                _theString = value;
            }
        }
    }
}
