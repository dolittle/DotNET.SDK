// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.SDK.Events.given
{
    public class an_event
    {
        public an_event(string a_string, int an_integer, bool a_bool)
        {
            this.a_string = a_string;
            this.an_integer = an_integer;
            this.a_bool = a_bool;
        }

        public string a_string { get; }

        public int an_integer { get; }

        public bool a_bool { get; }
    }
}
