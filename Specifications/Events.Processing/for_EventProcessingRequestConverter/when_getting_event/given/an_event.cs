// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Concepts;

namespace Dolittle.SDK.Events.Processing.for_EventProcessingRequestConverter.when_getting_event.given
{
    public class an_event : Value<an_event>
    {
        public string a_string { get; set; }

        public int an_integer { get; set; }

        public bool a_bool { get; set; }
    }
}